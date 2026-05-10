import { useEffect, useState } from 'react'
import { getMateriais, getTecnicos, createMaterial, updateMaterial, createTecnico, updateTecnico } from '../services/api'
import { Layout } from '../main'

const fmt = (v) => `R$ ${Number(v||0).toLocaleString('pt-BR',{minimumFractionDigits:2})}`

export default function Cadastros() {
  const [aba, setAba] = useState('materiais')
  const [materiais, setMateriais] = useState([])
  const [tecnicos, setTecnicos]   = useState([])
  const [editando, setEditando]   = useState(null)
  const [form, setForm]           = useState({})

const recarregar = async () => {
  getMateriais({ apenasAtivos: false }).then(r => {
    const sorted = [...r.data].sort((a, b) => {
      if (a.ativo !== b.ativo) return a.ativo ? -1 : 1
      return a.materialId - b.materialId
    })
    setMateriais(sorted)
  })
  getTecnicos({ apenasAtivos: false }).then(r => {
    const sorted = [...r.data].sort((a, b) => {
      if (a.ativo !== b.ativo) return a.ativo ? -1 : 1
      return a.tecnicoId - b.tecnicoId
    })
    setTecnicos(sorted)
  })
}

  useEffect(() => { recarregar() }, [])

  const salvarMaterial = async () => {
    try {
      if (editando === 'novo-mat') {
        await createMaterial({ nome: form.nome, unidade: form.unidade, precoUnitario: Number(form.precoUnitario) })
      } else {
        await updateMaterial(form.materialId, { nome: form.nome, unidade: form.unidade, precoUnitario: Number(form.precoUnitario), ativo: form.ativo })
      }
      await recarregar()
      setEditando(null)
    } catch (e) { alert('Erro: ' + (e.response?.data || e.message)) }
  }

  const salvarTecnico = async () => {
    try {
      if (editando === 'novo-tec') {
        await createTecnico({ nome: form.nome, cargo: form.cargo, valorHora: Number(form.valorHora) })
      } else {
        await updateTecnico(form.tecnicoId, { nome: form.nome, cargo: form.cargo, valorHora: Number(form.valorHora), ativo: form.ativo })
      }
      await recarregar()
      setEditando(null)
    } catch (e) { alert('Erro: ' + (e.response?.data || e.message)) }
  }

  return (
    <Layout titulo="Cadastros" breadcrumb="Início / Cadastros">
      <div className="tabs">
        <button className={`tab-btn ${aba==='materiais'?'active':''}`} onClick={() => setAba('materiais')}>🔩 Materiais</button>
        <button className={`tab-btn ${aba==='tecnicos'?'active':''}`}  onClick={() => setAba('tecnicos')}>👷 Técnicos</button>
      </div>

      {/* ── MATERIAIS ── */}
      {aba === 'materiais' && (
        <div className="card">
          <div className="card-header">
            <div className="card-title"><div className="card-title-icon">🔩</div> Catálogo de Materiais</div>
            <button className="btn btn-primary btn-sm"
              onClick={() => { setForm({ nome:'', unidade:'un', precoUnitario:'', ativo:true }); setEditando('novo-mat') }}>
              + Novo Material
            </button>
          </div>

          {editando && (editando === 'novo-mat' || editando.startsWith('mat-')) && (
            <div className="inline-form" style={{margin:'16px 20px 0'}}>
              <div style={{fontWeight:700,fontSize:13,color:'var(--accent)',marginBottom:12}}>
                {editando === 'novo-mat' ? '+ Novo Material' : '✏️ Editar Material'}
              </div>
              <div className="form-grid">
                <div className="form-group full">
                  <label>Nome</label>
                  <input value={form.nome||''} onChange={e => setForm({...form,nome:e.target.value})} placeholder="Ex: Disjuntor 20A" />
                </div>
                <div className="form-group">
                  <label>Unidade</label>
                  <select value={form.unidade||'un'} onChange={e => setForm({...form,unidade:e.target.value})}>
                    <option>un</option><option>m</option><option>pct</option><option>kg</option>
                  </select>
                </div>
                <div className="form-group">
                  <label>Preço Unitário (R$)</label>
                  <input type="number" min="0" step="0.01" value={form.precoUnitario||''} onChange={e => setForm({...form,precoUnitario:e.target.value})} />
                </div>
                {editando !== 'novo-mat' && (
                  <div className="form-group">
                    <label>Ativo</label>
                    <select value={form.ativo?'true':'false'} onChange={e => setForm({...form,ativo:e.target.value==='true'})}>
                      <option value="true">Sim</option><option value="false">Não</option>
                    </select>
                  </div>
                )}
              </div>
              <div style={{display:'flex',gap:8,marginTop:12}}>
                <button className="btn btn-primary btn-sm" onClick={salvarMaterial}>💾 Salvar</button>
                <button className="btn btn-secondary btn-sm" onClick={() => setEditando(null)}>Cancelar</button>
              </div>
            </div>
          )}

          <div className="table-wrap">
            <table>
              <thead><tr><th>#</th><th>Material</th><th>Unid.</th><th>Preço Unitário</th><th>Status</th><th>Ações</th></tr></thead>
              <tbody>
                {materiais.map(m => (
                    <tr key={m.materialId} style={!m.ativo ? { opacity: 0.6, background: '#fff5f5' } : {}}>
                    <td style={{color:'var(--text3)',fontWeight:600}}>{m.materialId}</td>
                    <td style={{fontWeight:500}}>{m.nome}</td>
                    <td style={{color:'var(--text2)'}}>{m.unidade}</td>
                    <td style={{fontFamily:'var(--mono)',fontSize:12}}>{fmt(m.precoUnitario)}</td>
                    <td><span className={`badge ${m.ativo?'Sim':'Nao'}`}>{m.ativo?'Ativo':'Inativo'}</span></td>
                    <td>
                        <div style={{ display: 'flex', gap: 6 }}>
                            <button className="btn btn-secondary btn-sm"
                                onClick={() => { setForm({ ...m }); setEditando(`mat-${m.materialId}`) }}>
                                ✏️ Editar
                            </button>
                            <button className="btn btn-sm"
                                style={{ background: m.ativo ? '#fef2f2' : '#f0fdf4', color: m.ativo ? 'var(--danger)' : 'var(--success)', border: 'none' }}
                                onClick={async () => {
                                    if (!confirm(m.ativo ? `Desativar "${m.nome}"?` : `Ativar "${m.nome}"?`)) return
                                    await updateMaterial(m.materialId, { ...m, ativo: !m.ativo })
                                    recarregar()
                                }}>
                                {m.ativo ? '🚫 Desativar' : '✅ Ativar'}
                            </button>
                        </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ── TÉCNICOS ── */}
      {aba === 'tecnicos' && (
        <div className="card">
          <div className="card-header">
            <div className="card-title"><div className="card-title-icon">👷</div> Técnicos</div>
            <button className="btn btn-primary btn-sm"
              onClick={() => { setForm({ nome:'', cargo:'Técnico Sênior', valorHora:'', ativo:true }); setEditando('novo-tec') }}>
              + Novo Técnico
            </button>
          </div>

          {editando && (editando === 'novo-tec' || editando.startsWith('tec-')) && (
            <div className="inline-form" style={{margin:'16px 20px 0'}}>
              <div style={{fontWeight:700,fontSize:13,color:'var(--accent)',marginBottom:12}}>
                {editando === 'novo-tec' ? '+ Novo Técnico' : '✏️ Editar Técnico'}
              </div>
              <div className="form-grid">
                <div className="form-group">
                  <label>Nome</label>
                  <input value={form.nome||''} onChange={e => setForm({...form,nome:e.target.value})} placeholder="Ex: Carlos Silva" />
                </div>
                <div className="form-group">
                  <label>Cargo</label>
                  <select value={form.cargo||''} onChange={e => setForm({...form,cargo:e.target.value})}>
                    <option>Técnico Júnior</option><option>Técnico Sênior</option><option>Engenheiro</option>
                  </select>
                </div>
                <div className="form-group">
                  <label>Valor/Hora (R$)</label>
                  <input type="number" min="0" step="0.01" value={form.valorHora||''} onChange={e => setForm({...form,valorHora:e.target.value})} />
                </div>
                {editando !== 'novo-tec' && (
                  <div className="form-group">
                    <label>Ativo</label>
                    <select value={form.ativo?'true':'false'} onChange={e => setForm({...form,ativo:e.target.value==='true'})}>
                      <option value="true">Sim</option><option value="false">Não</option>
                    </select>
                  </div>
                )}
              </div>
              <div style={{display:'flex',gap:8,marginTop:12}}>
                <button className="btn btn-primary btn-sm" onClick={salvarTecnico}>💾 Salvar</button>
                <button className="btn btn-secondary btn-sm" onClick={() => setEditando(null)}>Cancelar</button>
              </div>
            </div>
          )}

          <div className="table-wrap">
            <table>
              <thead><tr><th>#</th><th>Nome</th><th>Cargo</th><th>Valor/Hora</th><th>Status</th><th>Ações</th></tr></thead>
              <tbody>
                {tecnicos.map(t => (
                    <tr key={t.tecnicoId} style={!t.ativo ? { opacity: 0.6, background: '#fff5f5' } : {}}>
                    <td style={{color:'var(--text3)',fontWeight:600}}>{t.tecnicoId}</td>
                    <td style={{fontWeight:500}}>{t.nome}</td>
                    <td style={{color:'var(--text2)'}}>{t.cargo}</td>
                    <td style={{fontFamily:'var(--mono)',fontSize:12}}>{fmt(t.valorHora)}/h</td>
                    <td><span className={`badge ${t.ativo?'Sim':'Nao'}`}>{t.ativo?'Ativo':'Inativo'}</span></td>
                    <td>
                        <div style={{ display: 'flex', gap: 6 }}>
                            <button className="btn btn-secondary btn-sm"
                                onClick={() => { setForm({ ...t }); setEditando(`tec-${t.tecnicoId}`) }}>
                                ✏️ Editar
                            </button>
                            <button className="btn btn-sm"
                                style={{ background: t.ativo ? '#fef2f2' : '#f0fdf4', color: t.ativo ? 'var(--danger)' : 'var(--success)', border: 'none' }}
                                onClick={async () => {
                                    if (!confirm(t.ativo ? `Desativar "${t.nome}"?` : `Ativar "${t.nome}"?`)) return
                                    await updateTecnico(t.tecnicoId, { ...t, ativo: !t.ativo })
                                    recarregar()
                                }}>
                                {t.ativo ? '🚫 Desativar' : '✅ Ativar'}
                            </button>
                        </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </Layout>
  )
}
