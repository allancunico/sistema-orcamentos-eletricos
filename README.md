# ⚡ Sistema de Orçamentos — Quadros Elétricos

Stack: ASP.NET Core 8 Web API + React 18 + SQL Server Express

---

## Pré-requisitos

- .NET 8 SDK → https://dotnet.microsoft.com/download
- Node.js 20+ → https://nodejs.org
- SQL Server Express → https://www.microsoft.com/sql-server/sql-server-downloads
- (Opcional) Visual Studio 2022 ou VS Code

---

## 1. Banco de Dados — SQL Server Express

Após instalar o SQL Server Express, a instância padrão se chama `localhost\SQLEXPRESS`.

O banco **OrcamentosDB** é criado automaticamente na primeira execução via Entity Framework Migrations.

Se precisar ajustar a connection string, edite `backend/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=OrcamentosDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## 2. Backend — ASP.NET Core

```bash
cd backend

# Instalar dependências (EF Core, ClosedXML)
dotnet restore

# Criar a migration inicial
dotnet ef migrations add InitialCreate

# Aplicar migration e popular banco (seed automático)
dotnet ef database update

# Rodar a API (porta 5000)
dotnet run
```

A API ficará disponível em: http://localhost:5000  
Swagger UI (documentação): http://localhost:5000/swagger

> O seed já insere 21 materiais e 3 técnicos automaticamente.

---

## 3. Frontend — React

```bash
cd frontend

# Instalar dependências
npm install

# Rodar em desenvolvimento (porta 5173)
npm run dev
```

Abra no navegador: http://localhost:5173

O Vite já está configurado com proxy para `/api → http://localhost:5000`,
então não há problema de CORS em desenvolvimento.

---

## 4. Estrutura do Projeto

```
/backend
  /Controllers
    MateriaisController.cs    → GET/POST/PUT /api/materiais
    TecnicosController.cs     → GET/POST/PUT /api/tecnicos
    OrcamentosController.cs   → CRUD + exportação Excel
    DashboardController.cs    → indicadores e gráficos
  /Models
    Models.cs                 → Material, Tecnico, Orcamento, ItemOrcamento, MaoDeObra
  /DTOs
    DTOs.cs                   → todos os records de entrada/saída
  /Data
    AppDbContext.cs            → EF Core + seed
  /Services
    ExcelExportService.cs     → geração do .xlsx com ClosedXML
  Program.cs
  appsettings.json

/frontend/src
  /pages
    Dashboard.jsx             → gráficos e totais
    NovoOrcamento.jsx         → formulário novo
    EditarOrcamento.jsx       → edição de orçamento existente
    Historico.jsx             → listagem com filtros
    Cadastros.jsx             → gerenciar materiais e técnicos
  /components
    FormOrcamento.jsx         → formulário reutilizável (novo + edição)
  /services
    api.js                    → todas as chamadas HTTP (Axios)
  index.css                   → estilos globais
  main.jsx                    → rotas e layout
```

---

## 5. Deploy no Windows Server (IIS)

### Backend
```bash
dotnet publish -c Release -o ./publish
```
- Copie a pasta `publish` para o servidor, ex: `C:\inetpub\orcamentos\api`
- No IIS: crie um novo site apontando para essa pasta
- Instale o [.NET 8 Hosting Bundle](https://dotnet.microsoft.com/download) no servidor
- Application Pool: sem código gerenciado (No Managed Code)

### Frontend
```bash
npm run build
```
- Copie a pasta `dist` para o servidor, ex: `C:\inetpub\orcamentos\frontend`
- No IIS: crie um site apontando para `dist`
- Instale o módulo **URL Rewrite** no IIS
- Crie um arquivo `web.config` na pasta `dist`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="SPA" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
          </conditions>
          <action type="Rewrite" url="/index.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

---

## 6. Endpoints da API

| Método | Endpoint                        | Descrição                        |
|--------|---------------------------------|----------------------------------|
| GET    | /api/materiais                  | Lista materiais                  |
| POST   | /api/materiais                  | Novo material                    |
| PUT    | /api/materiais/{id}             | Atualiza material                |
| GET    | /api/tecnicos                   | Lista técnicos                   |
| POST   | /api/tecnicos                   | Novo técnico                     |
| PUT    | /api/tecnicos/{id}              | Atualiza técnico                 |
| GET    | /api/orcamentos                 | Lista orçamentos (com filtros)   |
| POST   | /api/orcamentos                 | Cria orçamento                   |
| GET    | /api/orcamentos/{id}            | Detalhe do orçamento             |
| PUT    | /api/orcamentos/{id}            | Edita orçamento                  |
| PATCH  | /api/orcamentos/{id}/status     | Atualiza apenas o status         |
| GET    | /api/orcamentos/{id}/exportar   | Baixa o .xlsx do orçamento       |
| GET    | /api/dashboard?ano=2026         | Dados do dashboard               |
