import axios from 'axios'

const api = axios.create({ baseURL: '/api' })

// Materiais
export const getMateriais = (params) => api.get('/materiais', { params })
export const createMaterial = (data) => api.post('/materiais', data)
export const updateMaterial = (id, data) => api.put(`/materiais/${id}`, data)

// Técnicos
export const getTecnicos = (params) => api.get('/tecnicos', { params })
export const createTecnico = (data) => api.post('/tecnicos', data)
export const updateTecnico = (id, data) => api.put(`/tecnicos/${id}`, data)

// Orçamentos
export const getOrcamentos = (params) => api.get('/orcamentos', { params })
export const getOrcamento = (id) => api.get(`/orcamentos/${id}`)
export const createOrcamento = (data) => api.post('/orcamentos', data)
export const updateOrcamento = (id, data) => api.put(`/orcamentos/${id}`, data)
export const atualizarStatus = (id, status) => api.patch(`/orcamentos/${id}/status`, JSON.stringify(status), {
  headers: { 'Content-Type': 'application/json' }
})
export const exportarOrcamento = async (id, nomeCliente) => {
  const res = await api.get(`/orcamentos/${id}/exportar`, { responseType: 'blob' })
  const url = URL.createObjectURL(res.data)
  const a = document.createElement('a')
  a.href = url
  a.download = `Orcamento_${String(id).padStart(4,'0')}_${nomeCliente}.xlsx`
  a.click()
  URL.revokeObjectURL(url)
}

// Dashboard
export const getDashboard = (ano) => api.get('/dashboard', { params: { ano } })
