import { useEffect, useState } from 'react'
import { getMateriais, getTecnicos } from '../services/api'

const fmt = (v) => `R$ ${Number(v || 0).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`

const getStep = (unidade) => {
  switch (unidade) {
    case 'm':   return '0.5'
    case 'un':  return '1'
    case 'pct': return '1'
    case 'kg':  return '0.1'
    default:    return '0.5'
  }
}

export default function FormOrcamento({ inicial, onSalvar, onExportar, carregando }) {
  const [materiais, setMateriais] = useState([])
  const [tecnicos, setTecnicos]   = useState([])
  const [cliente, setCliente]     = useState(inicial?.nomeCliente || '')
  const [descricao, setDescricao] = useState(inicial?.descricaoServico || '')
  const [dataOrc, setDataOrc]     = useState(inicial?.dataOrcamento || new Date().toISOString().split('T')[0])
  const [observacao, setObservacao] = useState(inicial?.observacao || '')
  const [status, setStatus]       = useState(inicial?.status || 'Rascunho')
  const [itens, setItens]         = useState(inicial?.itens?.map(i => ({
    materialId: i.materialId, quantidade: i.quantidade,
    nome: i.nomeMaterial, unidade: i.unidade, precoUnitario: i.precoUnitario
  })) || [])
  const [maoDeObra, setMaoDeObra] = useState(inicial?.maoDeObra?.map(m => ({
    tecnicoId: m.tecnicoId, horas: m.horas, numeroTecnicos: m.numeroTecnicos,
    nome: m.nomeTecnico, valorHora: m.valorHora
  })) || [])

  useEffect(() => {
    getMateriais().then(r => setMateriais(r.data))
    getTecnicos().then(r => setTecnicos(r.data))
  }, [])

  const totalMat = itens.reduce((a, i) => a + (i.quantidade * i.precoUnitario || 0), 0)
  const totalMO  = maoDeObra.reduce((a, m) => a + (m.horas * m.valorHora * m.numeroTecnicos || 0), 0)
  const total    = totalMat + totalMO
  const horas    = maoDeObra.reduce((a, m) => a + (Number(m.horas) || 0), 0)

  const addItem = () => {
    if (!materiais.length) return
    const m = materiais[0]
    setItens([...itens, { materialId: m.materialId, quantidade: 1, nome: m.nome, unidade: m.unidade, precoUnitario: m.precoUnitario }])
  }

  const updateItem = (idx, field, value) => {
    const next = [...itens]
    next[idx] = { ...next[idx], [field]: value }
    if (field === 'materialId') {
      const m = materiais.find(m => m.materialId === Number(value))
      if (m) next[idx] = { ...next[idx], nome: m.nome, unidade: m.unidade, precoUnitario: m.precoUnitario }
    }
    setItens(next)
  }

  const addMO = () => {
    if (!tecnicos.length) return
    const t = tecnicos[0]
    setMaoDeObra([...maoDeObra, { tecnicoId: t.tecnicoId, horas: 1, numeroTecnicos: 1, nome: t.nome, valorHora: t.valorHora }])
  }

  const updateMO = (idx, field, value) => {
    const next = [...maoDeObra]
    next[idx] = { ...next[idx], [field]: value }
    if (field === 'tecnicoId') {
      const t = tecnicos.find(t => t.tecnicoId === Number(value))
      if (t) next[idx] = { ...next[idx], nome: t.nome, valorHora: t.valorHora }
    }
    setMaoDeObra(next)
  }

  const handleSalvar = () => {
    if (!cliente.trim()) return alert('Informe o nome do cliente.')
    if (!descricao.trim()) return alert('Informe a descrição do serviço.')
    onSalvar({
      nomeCliente: cliente, descricaoServico: descricao, status,
      itens: itens.map(i => ({ materialId: Number(i.materialId), quantidade: Number(i.quantidade) })),
      maoDeObra: maoDeObra.map(m => ({ tecnicoId: Number(m.tecnicoId), horas: Number(m.horas), numeroTecnicos: Number(m.numeroTecnicos) }))
    })
  }

  return (
    <div className="two-col">
      {/* Coluna principal */}
      <div>
        {/* Informações */}
        <div className="card">
          <div className="card-header">
            <div className="card-title">
              <div className="card-title-icon">📋</div>
              Informações do Orçamento
            </div>
            <select value={status} onChange={e => setStatus(e.target.value)}
              style={{ padding: '5px 10px', borderRadius: 6, border: '1.5px solid var(--border)', fontSize: 12, fontWeight: 600, color: 'var(--text2)', background: 'var(--surface)' }}>
              <option>Rascunho</option><option>Enviado</option><option>Aprovado</option><option>Cancelado</option>
            </select>
          </div>
          <div className="card-body">
            <div className="form-grid">
              <div className="form-group">
                <label className="required">Cliente</label>
                <input value={cliente} onChange={e => setCliente(e.target.value)} placeholder="Ex: Empresa XYZ Ltda" />
              </div>
              <div className="form-group">
                <label>Data do Orçamento</label>
                <input type="date" value={dataOrc} onChange={e => setDataOrc(e.target.value)} />
              </div>
              <div className="form-group full">
                <label className="required">Título / Descrição do Serviço</label>
                <input value={descricao} onChange={e => setDescricao(e.target.value)}
                  placeholder="Ex: Manutenção do quadro de distribuição principal" />
              </div>
              <div className="form-group full">
                <label>Observações</label>
                <textarea value={observacao} onChange={e => setObservacao(e.target.value)}
                  placeholder="Observações gerais sobre o orçamento..." rows={2} />
              </div>
            </div>
          </div>
        </div>

        {/* Materiais */}
        <div className="card">
          <div className="card-header">
            <div className="card-title">
              <div className="card-title-icon">🔩</div>
              Itens do Orçamento
            </div>
            <button className="btn btn-primary btn-sm" onClick={addItem}>+ Adicionar Material</button>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>#</th><th>Material</th><th>Unid.</th><th>Preço Unit.</th><th>Qtd.</th><th>Total</th><th></th>
                </tr>
              </thead>
              <tbody>
                {itens.length === 0 && (
                  <tr><td colSpan={7} style={{ textAlign: 'center', color: 'var(--text3)', padding: 24 }}>Nenhum material adicionado.</td></tr>
                )}
                {itens.map((item, idx) => (
                  <tr key={idx}>
                    <td style={{ color: 'var(--text3)', fontWeight: 600 }}>{idx + 1}</td>
                    <td>
                      <select value={item.materialId} onChange={e => updateItem(idx, 'materialId', e.target.value)}
                        style={{ width: '100%', padding: '5px 8px', border: '1.5px solid var(--border)', borderRadius: 6, fontSize: 12 }}>
                        {materiais.map(m => <option key={m.materialId} value={m.materialId}>{m.nome}</option>)}
                      </select>
                    </td>
                    <td style={{ color: 'var(--text3)' }}>{item.unidade}</td>
                    <td style={{ fontFamily: 'var(--mono)', fontSize: 12 }}>{fmt(item.precoUnitario)}</td>
                    <td>
                      <input type="number" min={getStep(item.unidade)} step={getStep(item.unidade)} value={item.quantidade}
                        onChange={e => updateItem(idx, 'quantidade', e.target.value)}
                        style={{ width: 72, padding: '5px 8px', border: '1.5px solid var(--border)', borderRadius: 6, textAlign: 'center', fontSize: 13 }} />
                    </td>
                    <td style={{ fontWeight: 700, color: 'var(--accent)' }}>{fmt(item.quantidade * item.precoUnitario)}</td>
                    <td>
                      <button onClick={() => setItens(itens.filter((_, i) => i !== idx))}
                        style={{ background: '#fef2f2', color: 'var(--danger)', border: 'none', borderRadius: 6, padding: '5px 8px', cursor: 'pointer', fontSize: 13 }}>✕</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        {/* Mão de Obra */}
        <div className="card">
          <div className="card-header">
            <div className="card-title">
              <div className="card-title-icon">👷</div>
              Mão de Obra
            </div>
            <button className="btn btn-primary btn-sm" onClick={addMO}>+ Adicionar Técnico</button>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr><th>#</th><th>Técnico</th><th>Valor/Hora</th><th>Horas</th><th>Nº Técnicos</th><th>Total</th><th></th></tr>
              </thead>
              <tbody>
                {maoDeObra.length === 0 && (
                  <tr><td colSpan={7} style={{ textAlign: 'center', color: 'var(--text3)', padding: 24 }}>Nenhum técnico adicionado.</td></tr>
                )}
                {maoDeObra.map((mo, idx) => (
                  <tr key={idx}>
                    <td style={{ color: 'var(--text3)', fontWeight: 600 }}>{idx + 1}</td>
                    <td>
                      <select value={mo.tecnicoId} onChange={e => updateMO(idx, 'tecnicoId', e.target.value)}
                        style={{ width: '100%', padding: '5px 8px', border: '1.5px solid var(--border)', borderRadius: 6, fontSize: 12 }}>
                        {tecnicos.map(t => <option key={t.tecnicoId} value={t.tecnicoId}>{t.nome} — {t.cargo}</option>)}
                      </select>
                    </td>
                    <td style={{ fontFamily: 'var(--mono)', fontSize: 12, color: 'var(--text2)' }}>{fmt(mo.valorHora)}/h</td>
                    <td>
                      <input type="number" min="0.5" step="0.5" value={mo.horas}
                        onChange={e => updateMO(idx, 'horas', e.target.value)}
                        style={{ width: 68, padding: '5px 8px', border: '1.5px solid var(--border)', borderRadius: 6, textAlign: 'center', fontSize: 13 }} />
                    </td>
                    <td style={{ fontWeight: 700, color: 'var(--accent)' }}>{fmt(mo.horas * mo.valorHora * mo.numeroTecnicos)}</td>
                    <td>
                      <button onClick={() => setMaoDeObra(maoDeObra.filter((_, i) => i !== idx))}
                        style={{ background: '#fef2f2', color: 'var(--danger)', border: 'none', borderRadius: 6, padding: '5px 8px', cursor: 'pointer', fontSize: 13 }}>✕</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* Painel Lateral */}
      <div style={{ position: 'sticky', top: 'calc(var(--topbar-h) + 24px)' }}>
        <div className="resumo-panel">
          <div style={{ fontSize: 12, fontWeight: 700, color: 'rgba(255,255,255,0.5)', textTransform: 'uppercase', letterSpacing: 1, marginBottom: 14 }}>
            Resumo do Orçamento
          </div>
          <div className="resumo-row">
            <span className="rlabel">Subtotal Materiais</span>
            <span className="rvalor">{fmt(totalMat)}</span>
          </div>
          <div className="resumo-row">
            <span className="rlabel">Subtotal Mão de Obra</span>
            <span className="rvalor">{fmt(totalMO)}</span>
          </div>
          <div className="resumo-total">
            <span className="rlabel">Total do Orçamento</span>
            <span className="rvalor">{fmt(total)}</span>
          </div>
          <div className="horas-badge">
            ⏱ Tempo estimado: {horas.toFixed(1)} horas
          </div>
        </div>

        <div style={{ marginTop: 12, display: 'flex', flexDirection: 'column', gap: 8 }}>
          <button className="btn btn-primary" style={{ width: '100%', justifyContent: 'center' }}
            onClick={handleSalvar} disabled={carregando}>
            {carregando ? '⏳ Salvando...' : '💾 Salvar Orçamento'}
          </button>
          {onExportar && (
            <button className="btn btn-success" style={{ width: '100%', justifyContent: 'center' }} onClick={onExportar}>
              ⬇️ Exportar Excel
            </button>
          )}
          <button className="btn btn-secondary" style={{ width: '100%', justifyContent: 'center' }}
            onClick={() => window.history.back()}>
            Cancelar
          </button>
        </div>
      </div>
    </div>
  )
}
