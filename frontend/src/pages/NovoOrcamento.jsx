import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { createOrcamento } from '../services/api'
import { Layout } from '../main'
import FormOrcamento from '../components/FormOrcamento'

export default function NovoOrcamento() {
  const [carregando, setCarregando] = useState(false)
  const navigate = useNavigate()

  const salvar = async (dados) => {
    setCarregando(true)
    try {
      await createOrcamento(dados)
      alert('Orçamento salvo com sucesso!')
      navigate('/historico')
    } catch (e) {
      alert('Erro ao salvar: ' + (e.response?.data || e.message))
    } finally {
      setCarregando(false)
    }
  }

  return (
    <Layout titulo="Criar Orçamento" breadcrumb="Início / Orçamentos / Novo">
      <FormOrcamento onSalvar={salvar} carregando={carregando} />
    </Layout>
  )
}
