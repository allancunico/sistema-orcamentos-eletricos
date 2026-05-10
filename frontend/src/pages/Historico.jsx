import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getOrcamentos, atualizarStatus, exportarOrcamento } from '../services/api'
import { Layout } from '../main'

const fmt = (v) => `R$ ${Number(v||0).toLocaleString('pt-BR',{minimumFractionDigits:2})}`

export default function Historico() {
  const [orcamentos, setOrcamentos] = useState([])
  const [filtroStatus, setFiltroStatus] = useState('')
  const [filtroCliente, setFiltroCliente] = useState('')
  const navigate = useNavigate()

  const carregar = () => {
    const params = {}
    if (filtroStatus)  params.status  = filtroStatus
    if (filtroCliente) params.cliente = filtroCliente
    getOrcamentos(params).then(r => setOrcamentos(r.data))
  }

  useEffect(() => { carregar() }, [])

    const mudarStatus = async (id, novoStatus, nomeCliente) => {
        if (!confirm(`Alterar status do orçamento de "${nomeCliente}" para "${novoStatus}"?`)) return
        try {
            await atualizarStatus(id, novoStatus)
            carregar()
        } catch (e) {
            alert('Erro ao alterar status: ' + (e.response?.data || e.message))
        }
    }

  return (
    <Layout titulo="Histórico de Orçamentos" breadcrumb="Início / Orçamentos"
      actions={<button className="btn btn-primary" onClick={() => navigate('/orcamentos/novo')}>+ Novo Orçamento</button>}>

      <div className="card">
        <div className="filter-bar">
          <div className="form-group">
            <label>Cliente</label>
            <input value={filtroCliente} onChange={e => setFiltroCliente(e.target.value)}
              placeholder="Buscar por cliente..." style={{width:200}} />
          </div>
          <div className="form-group">
            <label>Status</label>
            <select value={filtroStatus} onChange={e => setFiltroStatus(e.target.value)} style={{width:140}}>
              <option value="">Todos</option>
              <option>Rascunho</option><option>Enviado</option><option>Aprovado</option><option>Cancelado</option>
            </select>
          </div>
          <button className="btn btn-primary" onClick={carregar}>🔍 Filtrar</button>
          <button className="btn btn-secondary" onClick={() => { setFiltroStatus(''); setFiltroCliente(''); setTimeout(carregar,100) }}>Limpar</button>
        </div>

        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Nº</th><th>Cliente</th><th>Serviço</th><th>Data</th>
                <th>Horas</th><th>Total</th><th>Status</th><th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {orcamentos.length === 0 && (
                <tr><td colSpan={8} style={{textAlign:'center',color:'var(--text3)',padding:32}}>Nenhum orçamento encontrado.</td></tr>
              )}
              {orcamentos.map(o => (
                <tr key={o.orcamentoId}>
                  <td style={{fontWeight:700,color:'var(--accent)',fontFamily:'var(--mono)',fontSize:12}}>
                    ORC-{String(o.orcamentoId).padStart(4,'0')}
                  </td>
                  <td style={{fontWeight:500}}>{o.nomeCliente}</td>
                  <td style={{maxWidth:180,overflow:'hidden',textOverflow:'ellipsis',whiteSpace:'nowrap',color:'var(--text2)'}}>
                    {o.descricaoServico}
                  </td>
                  <td style={{color:'var(--text2)',whiteSpace:'nowrap'}}>
                    {new Date(o.criadoEm).toLocaleDateString('pt-BR')}
                  </td>
                  <td style={{color:'var(--text2)'}}>{Number(o.horasEstimadas).toFixed(1)}h</td>
                  <td style={{fontWeight:700,fontFamily:'var(--mono)',fontSize:12}}>{fmt(o.totalGeral)}</td>
                  <td><span className={`badge ${o.status}`}>{o.status}</span></td>
                  <td>
                    <div style={{display:'flex',gap:6,alignItems:'center'}}>
                      <button className="btn btn-secondary btn-sm"
                        onClick={() => navigate(`/orcamentos/${o.orcamentoId}/editar`)}>✏️ Editar</button>
                      <button className="btn btn-success btn-sm"
                        onClick={() => exportarOrcamento(o.orcamentoId, o.nomeCliente)}>⬇️ XLS</button>
                      <select value={o.status} onChange={e => mudarStatus(o.orcamentoId, e.target.value, o.nomeCliente)}
                        style={{fontSize:11,padding:'5px 6px',borderRadius:6,border:'1.5px solid var(--border)',background:'var(--surface)',color:'var(--text2)'}}>
                        <option>Rascunho</option><option>Enviado</option><option>Aprovado</option><option>Cancelado</option>
                      </select>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </Layout>
  )
}
