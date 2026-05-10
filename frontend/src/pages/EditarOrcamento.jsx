import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { getOrcamento, updateOrcamento, exportarOrcamento } from '../services/api'
import { Layout } from '../main'
import FormOrcamento from '../components/FormOrcamento'

export default function EditarOrcamento() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [orc, setOrc] = useState(null)
  const [carregando, setCarregando] = useState(false)

  useEffect(() => { getOrcamento(id).then(r => setOrc(r.data)) }, [id])

  const salvar = async (dados) => {
    setCarregando(true)
    try {
      await updateOrcamento(id, dados)
      alert('Orçamento atualizado!')
      navigate('/historico')
    } catch (e) {
      alert('Erro: ' + (e.response?.data || e.message))
    } finally { setCarregando(false) }
  }

  if (!orc) return <Layout titulo="Editar Orçamento"><p style={{color:'var(--text3)'}}>Carregando...</p></Layout>

  return (
    <Layout titulo={`Editar ORC-${String(id).padStart(4,'0')}`} breadcrumb="Início / Orçamentos / Editar">
      <FormOrcamento inicial={orc} onSalvar={salvar}
        onExportar={() => exportarOrcamento(id, orc.nomeCliente)} carregando={carregando} />
    </Layout>
  )
}
