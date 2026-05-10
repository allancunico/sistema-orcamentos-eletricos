import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter, Routes, Route, NavLink } from 'react-router-dom'
import './index.css'
import Dashboard from './pages/Dashboard'
import Historico from './pages/Historico'
import NovoOrcamento from './pages/NovoOrcamento'
import EditarOrcamento from './pages/EditarOrcamento'
import Cadastros from './pages/Cadastros'

export function Layout({ children, titulo, breadcrumb, actions }) {
  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="sidebar-logo">
          <span style={{ fontSize: 24, lineHeight: 1 }}>⚡</span>
          <div>
            <div className="sidebar-logo-name" style={{ color: '#6aaa1e', fontSize: 15, letterSpacing: '-0.3px' }}>OrcaElétrico</div>
            <div className="sidebar-logo-sub">Sistema de Orçamentos</div>
          </div>
        </div>
        <nav>
          <NavLink to="/" end>
            <span className="nav-icon">▣</span> Dashboard
          </NavLink>
          <NavLink to="/orcamentos/novo">
            <span className="nav-icon">＋</span> Novo Orçamento
          </NavLink>
          <NavLink to="/historico">
            <span className="nav-icon">≡</span> Histórico
          </NavLink>
          <NavLink to="/cadastros">
            <span className="nav-icon">⚙</span> Cadastros
          </NavLink>
        </nav>    
      </aside>
      <div className="main">
        <div className="topbar">
          <div>
            {breadcrumb && <div className="topbar-breadcrumb">{breadcrumb}</div>}
            <div className="topbar-title">{titulo}</div>
          </div>
          <div className="topbar-right">{actions}</div>
        </div>
        <div className="page">{children}</div>
      </div>
    </div>
  )
}

ReactDOM.createRoot(document.getElementById('root')).render(
  <BrowserRouter>
    <Routes>
      <Route path="/" element={<Dashboard />} />
      <Route path="/orcamentos/novo" element={<NovoOrcamento />} />
      <Route path="/orcamentos/:id/editar" element={<EditarOrcamento />} />
      <Route path="/historico" element={<Historico />} />
      <Route path="/cadastros" element={<Cadastros />} />
    </Routes>
  </BrowserRouter>
)
