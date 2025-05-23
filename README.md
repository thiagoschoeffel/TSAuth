# TSAuth

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/docker-ready-blue)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

TSAuth é um microserviço de autenticação com 
geração de JWT e refresh token, desenvolvido 
em .NET 9, com persistência em SQLServer. 
Ele expõe rotas para login e renovação de 
token e está preparado para integração com outros 
microserviços.

---

## 🚀 Tecnologias utilizadas

- .NET 9
- ASP.NET Core Web API
- SQLServer
- Entity Framework Core
- JWT Bearer Authentication
- Swagger UI

---

## 📁 Estrutura do projeto

```plaintext
TSAuth/
├── TSAuth.Api/
│   ├── Application/
│   ├── Configurations/
│   ├── Contracts/
│   ├── Controllers/
│   ├── Infrastructure/
│   ├── Migrations/
│   ├── Models/
│   ├── appsetting/s.Template.json
│   └── Program.cs
├── Dockerfile
├── docker-compose.yml
├── README.md
└── .gitignore
```

---

## ⚙️ Configuração local

### 1. Clone o repositório

```bash
git clone https://github.com/thiagoschoeffel/TSAuth.git
cd TSAuth
```

### 2. Copie o arquivo de configuração

```bash
cp TSAuth.Api/appsettings.Template.json TSAuth.Api/appsettings.json
```

Edite o `appsettings.json` com suas configurações locais:

- ConnectionString do PostgreSQL
- Nome, E-mail e Senha do usuário padrão criado na primeira execução do projeto
- Configurações para geração dos tokens JWT

### 3. Suba os containers com Docker

```bash
docker-compose up -d
```

### 4. Execute o projeto

```bash
cd TSAuth.Api
dotnet run
```

---

## 🧪 Testando a API

Acesse a documentação interativa Swagger em:

```
https://localhost:PORT/swagger
```

Endpoints disponíveis:

- `POST /api/v1/auth` → Geração de JWT
- `POST /api/v1/auth/refresh` → Renovação do token

---

## 🔐 Admin padrão (definido via appsettings)

- **Email:** `Definido no appsettings.json`
- **Senha:** `Definido no appsettings.json`

---

## 📄 Licença

Este projeto está licenciado sob a licença MIT.
