# 🌱 Cibra - Sistema de Análise Inteligente de Postagens Agrícolas

Solução fullstack para análise automática de postagens agrícolas utilizando IA generativa.

## 📋 Visão Geral

Sistema desenvolvido como parte do teste técnico para Desenvolvedor Fullstack Sênior na Cibra. A aplicação permite que usuários criem postagens sobre atividades agrícolas e recebam análises automáticas de IA com insights sobre:

- Tipo de cultura mencionada
- Estágio do cultivo (plantio, crescimento, colheita)
- Problemas identificados (pragas, clima, solo)
- Recomendações técnicas

Além disso, os usuários podem mencionar `@AssistenteIA` para obter análises específicas e interagir com o histórico completo.

## 🛠️ Stack Tecnológica

### Backend
- **.NET 9** com ASP.NET Core Web API
- **Entity Framework Core** para SQL Server
- **Clean Architecture** (Domain, Application, Infrastructure, API)
- **CQRS Pattern** para separação de comandos e consultas
- **Repository Pattern** e **Unit of Work**
- **Polly** para retry policies e resiliência

### Frontend
- **Next.js 14** (App Router)
- **TypeScript**
- **Tailwind CSS**
- **Zustand** para gerenciamento de estado
- **Axios** para chamadas HTTP
- **React Hot Toast** para notificações
- **Lucide React** para ícones
- **date-fns** para formatação de datas

### Infraestrutura
- **Docker** e **Docker Compose**
- **SQL Server 2022**
- **MongoDB 7.0** (opcional, para histórico estendido)
- **OpenAI API** (GPT-4o-mini)

## 📁 Estrutura do Projeto

```
cibra-agricultural-posts/
├── backend/
│   ├── Domain/
│   │   ├── Entities/
│   │   │   └── Post.cs
│   │   └── Interfaces/
│   │       ├── IPostRepository.cs
│   │       ├── IUnitOfWork.cs
│   │       └── IAIService.cs
│   ├── Application/
│   │   ├── DTOs/
│   │   │   └── PostDTOs.cs
│   │   ├── Commands/
│   │   │   └── CreatePostCommand.cs
│   │   └── Queries/
│   │       └── PostQueries.cs
│   ├── Infrastructure/
│   │   ├── Data/
│   │   │   └── AppDbContext.cs
│   │   ├── Repositories/
│   │   │   └── PostRepository.cs
│   │   └── Services/
│   │       └── OpenAIService.cs
│   └── API/
│       ├── Controllers/
│       │   └── PostsController.cs
│       ├── Program.cs
│       ├── appsettings.json
│       ├── Dockerfile
│       └── Cibra.AgriculturalPosts.API.csproj
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── layout.tsx
│   │   │   ├── page.tsx
│   │   │   └── globals.css
│   │   ├── components/
│   │   │   ├── PostCard.tsx
│   │   │   ├── CreatePostForm.tsx
│   │   │   └── PostDetail.tsx
│   │   ├── lib/
│   │   │   └── api.ts
│   │   ├── store/
│   │   │   └── usePostStore.ts
│   │   └── types/
│   │       └── post.ts
│   ├── package.json
│   ├── tsconfig.json
│   ├── tailwind.config.ts
│   ├── next.config.js
│   └── Dockerfile
├── docker-compose.yml
├── .env.example
├── .gitignore
└── README.md
```

## 🚀 Instalação e Execução

### Pré-requisitos

- Docker e Docker Compose instalados
- Chave API da OpenAI ([obter aqui](https://platform.openai.com/api-keys))

### Passo 1: Clonar o Repositório

```bash
git clone https://github.com/seu-usuario/cibra-agricultural-posts.git
cd cibra-agricultural-posts
```

### Passo 2: Configurar Variáveis de Ambiente

```bash
cp .env.example .env
```

Edite o arquivo `.env` e configure sua chave da OpenAI:

```bash
OPENAI_API_KEY=sk-your-actual-api-key-here
```

### Passo 3: Iniciar com Docker Compose

```bash
docker-compose up --build
```

### Passo 4: Acessar a Aplicação

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

## 🎯 Funcionalidades Implementadas

### ✅ Requisitos Obrigatórios

- [x] Backend em .NET 9 com ASP.NET Core Web API
- [x] Clean Architecture com separação de camadas
- [x] Entity Framework Core com SQL Server
- [x] Repository Pattern e Unit of Work
- [x] CQRS Pattern
- [x] Frontend em Next.js 14 com App Router
- [x] TypeScript no frontend
- [x] Tailwind CSS para estilização
- [x] Integração com OpenAI API
- [x] Análise automática de postagens
- [x] Identificação de cultura, estágio e problemas
- [x] Recomendações técnicas
- [x] Menção @AssistenteIA para análises específicas
- [x] Armazenamento de histórico de interações
- [x] Docker e Docker Compose
- [x] Tratamento de erros e retry policies
- [x] Validações de entrada

### 🎨 Diferenciais Implementados

- [x] Gerenciamento de estado com Zustand
- [x] Sistema de notificações (React Hot Toast)
- [x] Paginação de postagens
- [x] UI/UX moderna e responsiva
- [x] Loading states e feedback visual
- [x] Health checks
- [x] Swagger/OpenAPI documentation
- [x] Async AI analysis (fire and forget)
- [x] Confidence score nas análises

## 🔧 Configuração Manual (sem Docker)

### Backend

```bash
cd backend/API

# Instalar dependências
dotnet restore

# Configurar connection string no appsettings.json
# Executar migrations
dotnet ef database update

# Executar aplicação
dotnet run
```

### Frontend

```bash
cd frontend

# Instalar dependências
npm install

# Criar arquivo .env.local
echo "NEXT_PUBLIC_API_URL=http://localhost:5000" > .env.local

# Executar aplicação
npm run dev
```

## 📡 Endpoints da API

### Posts

- `GET /api/posts` - Listar todas as postagens (com paginação)
- `GET /api/posts/{id}` - Obter postagem por ID
- `GET /api/posts/user/{userId}` - Listar postagens de um usuário
- `POST /api/posts` - Criar nova postagem
- `PUT /api/posts/{id}` - Atualizar postagem
- `POST /api/posts/{id}/mention` - Mencionar @AssistenteIA

### Health & Docs

- `GET /health` - Health check
- `GET /swagger` - Documentação interativa

## 🧪 Testando a Aplicação

### 1. Criar uma Postagem

Acesse http://localhost:3000 e crie uma postagem de teste:

```
Plantei 50 hectares de soja hoje na Fazenda Santa Cruz, RS. 
O solo está bem preparado, mas notei algumas áreas com compactação. 
O clima está favorável para o desenvolvimento inicial, mas há previsão 
de chuvas intensas para a próxima semana. Vi alguns sinais de lagarta 
nas bordas da lavoura.
```

### 2. Aguardar Análise da IA

A análise será processada automaticamente em background. Aguarde alguns segundos e atualize a página para ver os resultados.

### 3. Interagir com @AssistenteIA

Clique na postagem para abrir os detalhes e faça perguntas como:

```
@AssistenteIA, qual o melhor método para controlar a lagarta?
@AssistenteIA, como devo preparar o solo para lidar com as chuvas?
@AssistenteIA, crie uma tabela com cronograma de manejo
```

## 🔐 Segurança e Boas Práticas

- Variáveis de ambiente para dados sensíveis
- Validação de entrada em todos os endpoints
- CORS configurado adequadamente
- Retry policies para chamadas externas
- Health checks para monitoramento
- Tratamento consistente de erros
- Logging estruturado

## 📊 Arquitetura

### Clean Architecture

```
┌─────────────────────────────────────┐
│           API Layer                 │
│  (Controllers, Middleware)          │
└─────────────┬───────────────────────┘
              │
┌─────────────▼───────────────────────┐
│      Application Layer              │
│  (Commands, Queries, DTOs)          │
└─────────────┬───────────────────────┘
              │
┌─────────────▼───────────────────────┐
│        Domain Layer                 │
│  (Entities, Interfaces)             │
└─────────────┬───────────────────────┘
              │
┌─────────────▼───────────────────────┐
│     Infrastructure Layer            │
│  (Data, Repositories, Services)     │
└─────────────────────────────────────┘
```

### Fluxo de Dados

```
Frontend (Next.js)
    ↓ HTTP Request
API Controller
    ↓ Command/Query
Command/Query Handler
    ↓ Domain Logic
Repository (via Unit of Work)
    ↓ EF Core
SQL Server Database

    ↓ Async (Fire & Forget)
AI Service → OpenAI API
    ↓ Analysis Result
Update Post with Analysis
```

## 🐛 Troubleshooting

### Problema: API não conecta ao SQL Server

```bash
# Verificar se o SQL Server está rodando
docker ps

# Verificar logs do SQL Server
docker logs cibra-sqlserver

# Aguardar o health check passar (pode levar até 30s)
```

### Problema: OpenAI API retorna erro 429

Isso indica rate limiting. O sistema tem retry policy automático, mas se persistir:

1. Aguarde alguns segundos entre requisições
2. Verifique se sua chave API tem créditos
3. Considere usar o modelo `gpt-3.5-turbo` (mais barato) no `appsettings.json`

### Problema: Frontend não carrega dados

```bash
# Verificar se a API está respondendo
curl http://localhost:5000/health

# Verificar variável de ambiente
echo $NEXT_PUBLIC_API_URL
```

## 📝 Melhorias Futuras

Se eu tivesse mais tempo, implementaria:

- [ ] Autenticação e autorização (JWT)
- [ ] SignalR para notificações em tempo real
- [ ] Testes unitários e de integração
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Kubernetes manifests para AKS
- [ ] Application Insights para telemetria
- [ ] RAG com histórico estendido
- [ ] Streaming de respostas da IA (SSE)
- [ ] Feature flags
- [ ] Dashboard admin
- [ ] Sistema de feedback de qualidade
- [ ] Suporte a múltiplos idiomas
- [ ] Upload de imagens das lavouras
- [ ] Integração com dados meteorológicos
- [ ] Relatórios e análises agregadas

### Principais Desafios

1. **Clean Architecture em .NET 9**: Adaptação para a nova versão com minimal APIs e top-level statements
2. **Async AI Processing**: Implementar fire-and-forget seguro sem bloquear a criação de posts
3. **Parsing de JSON da IA**: Tratamento robusto de respostas variadas do GPT
4. **State Management no Next.js 14**: Zustand com App Router e Server Components

## 📞 Contato

Para dúvidas sobre a implementação:

- GitHub: vinicius-bonetto
- Email: vini.bonetto10@gmail.com

---

## 🌱 Cibra

**Fazemos pela sociedade. Fazemos pelo planeta.**  
**Fertilizamos parcerias para alimentar e transformar vidas.**
