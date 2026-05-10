import { useEffect, useState } from 'react'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts'
import { getDashboard } from '../services/api'
import { Layout } from '../main'

const PIE_COLORS = ['#22c55e','#3b82f6','#f59e0b','#ef4444']
const fmt = (v) => `R$ ${Number(v||0).toLocaleString('pt-BR',{minimumFractionDigits:2})}`

export default function Dashboard() {
  const [data, setData] = useState(null)
  const [ano, setAno] = useState(new Date().getFullYear())

    useEffect(() => {
    getDashboard(ano)
        .then(r => setData(r.data))
        .catch(e => alert('Erro no dashboard: ' + (e.response?.data || e.message)))
    }, [ano])

  if (!data) return <Layout titulo="Dashboard"><p style={{color:'var(--text3)'}}>Carregando...</p></Layout>

  const statusData = [
    { name: 'Aprovado',  value: data.orcamentosAprovados },
    { name: 'Enviado',   value: data.orcamentosEnviados },
    { name: 'Rascunho',  value: data.orcamentosRascunho },
    { name: 'Cancelado', value: data.orcamentosCancelados },
  ].filter(s => s.value > 0)

  return (
    <Layout titulo="Dashboard" breadcrumb="Início / Dashboard"
      actions={
        <select value={ano} onChange={e => setAno(e.target.value)}
          style={{ padding: '7px 12px', borderRadius: 8, border: '1.5px solid var(--border)', fontSize: 13, fontWeight: 600, background: 'var(--surface)' }}>
          {[2024,2025,2026,2027].map(a => <option key={a}>{a}</option>)}
        </select>
      }>

      <div className="stats-grid">
        <div className="stat-card blue">
          <div className="stat-icon">📋</div>
          <div className="stat-label">Total de Orçamentos</div>
          <div className="stat-value">{data.totalOrcamentos}</div>
          <div className="stat-sub">em {ano}</div>
        </div>
        <div className="stat-card green">
          <div className="stat-icon">✅</div>
          <div className="stat-label">Valor Aprovado</div>
          <div className="stat-value" style={{fontSize:18}}>{fmt(data.valorTotalAprovado)}</div>
          <div className="stat-sub">{data.orcamentosAprovados} orçamentos aprovados</div>
        </div>
        <div className="stat-card amber">
          <div className="stat-icon">💰</div>
          <div className="stat-label">Valor Total Orçado</div>
          <div className="stat-value" style={{fontSize:18}}>{fmt(data.valorTotalOrcado)}</div>
          <div className="stat-sub">todos os status</div>
        </div>
        <div className="stat-card purple">
          <div className="stat-icon">📈</div>
          <div className="stat-label">Taxa de Aprovação</div>
          <div className="stat-value">
            {data.totalOrcamentos > 0 ? Math.round((data.orcamentosAprovados/data.totalOrcamentos)*100)+'%' : '—'}
          </div>
          <div className="stat-sub">{data.orcamentosEnviados} aguardando resposta</div>
        </div>
      </div>

      <div style={{display:'grid',gridTemplateColumns:'2fr 1fr',gap:20,marginBottom:20}}>
        <div className="card">
          <div className="card-header">
            <div className="card-title"><div className="card-title-icon">📊</div> Orçamentos por Mês — {ano}</div>
          </div>
          <div className="card-body">
            <ResponsiveContainer width="100%" height={220}>
              <BarChart data={data.porMes} margin={{top:5,right:10,bottom:5,left:0}}>
                <XAxis dataKey="mes" tick={{fontSize:11,fill:'var(--text3)'}} axisLine={false} tickLine={false} />
                <YAxis tick={{fontSize:11,fill:'var(--text3)'}} axisLine={false} tickLine={false} />
                <Tooltip contentStyle={{borderRadius:8,border:'1px solid var(--border)',fontSize:12}} />
                <Bar dataKey="quantidade" name="Orçamentos" fill="var(--accent)" radius={[4,4,0,0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div className="card">
          <div className="card-header">
            <div className="card-title"><div className="card-title-icon">🥧</div> Por Status</div>
          </div>
          <div className="card-body">
            {statusData.length > 0 ? (
              <ResponsiveContainer width="100%" height={220}>
                <PieChart>
                  <Pie data={statusData} dataKey="value" cx="50%" cy="50%" outerRadius={80} innerRadius={40}
                    label={({name,percent}) => `${(percent*100).toFixed(0)}%`} labelLine={false}>
                    {statusData.map((_,i) => <Cell key={i} fill={PIE_COLORS[i%PIE_COLORS.length]} />)}
                  </Pie>
                  <Tooltip />
                </PieChart>
              </ResponsiveContainer>
            ) : <p style={{textAlign:'center',color:'var(--text3)',paddingTop:80}}>Sem dados</p>}
            <div style={{display:'flex',flexWrap:'wrap',gap:8,marginTop:8}}>
              {statusData.map((d,i) => (
                <div key={i} style={{display:'flex',alignItems:'center',gap:5,fontSize:11,color:'var(--text2)'}}>
                  <div style={{width:8,height:8,borderRadius:2,background:PIE_COLORS[i%PIE_COLORS.length]}} />
                  {d.name} ({d.value})
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {data.materiaisMaisUsados.length > 0 && (
        <div className="card">
          <div className="card-header">
            <div className="card-title"><div className="card-title-icon">🔩</div> Materiais Mais Utilizados</div>
          </div>
          <div className="table-wrap">
            <table>
              <thead><tr><th>#</th><th>Material</th><th>Quantidade Total</th></tr></thead>
              <tbody>
                {data.materiaisMaisUsados.map((m,i) => (
                  <tr key={i}>
                    <td style={{color:'var(--text3)',fontWeight:700}}>{i+1}</td>
                    <td style={{fontWeight:500}}>{m.nome}</td>
                    <td>{Number(m.quantidadeTotal).toLocaleString('pt-BR')}</td>
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
