# 📁 Estrutura Completa de Pastas do Projeto

```
cibra-agricultural-posts/
│
├── 📂 backend/
│   │
│   ├── 📂 Domain/                          # Camada de Domínio (Regras de Negócio)
│   │   ├── 📂 Entities/
│   │   │   └── 📄 Post.cs                 # Entidade principal com regras de negócio
│   │   │
│   │   └── 📂 Interfaces/
│   │       ├── 📄 IPostRepository.cs       # Interface do repositório
│   │       ├── 📄 IUnitOfWork.cs          # Interface do Unit of Work
│   │       └── 📄 IAIService.cs           # Interface do serviço de IA
│   │
│   ├── 📂 Application/                     # Camada de Aplicação (Use Cases)
│   │   ├── 📂 DTOs/
│   │   │   └── 📄 PostDTOs.cs             # Data Transfer Objects
│   │   │
│   │   ├── 📂 Commands/
│   │   │   └── 📄 CreatePostCommand.cs     # Commands com handlers (CQRS)
│   │   │
│   │   └── 📂 Queries/
│   │       └── 📄 PostQueries.cs           # Queries com handlers (CQRS)
│   │
│   ├── 📂 Infrastructure/                  # Camada de Infraestrutura
│   │   ├── 📂 Data/
│   │   │   └── 📄 AppDbContext.cs         # Context do Entity Framework
│   │   │
│   │   ├── 📂 Repositories/
│   │   │   └── 📄 PostRepository.cs        # Implementação do repositório
│   │   │
│   │   └── 📂 Services/
│   │       └── 📄 OpenAIService.cs         # Implementação do serviço de IA
│   │
│   └── 📂 API/                             # Camada de Apresentação (Web API)
│       ├── 📂 Controllers/
│       │   └── 📄 PostsController.cs       # Controller REST
│       │
│       ├── 📄 Program.cs                   # Ponto de entrada e configuração DI
│       ├── 📄 appsettings.json            # Configurações da aplicação
│       ├── 📄 appsettings.Development.json
│       ├── 📄 Dockerfile                   # Docker para backend
│       └── 📄 Cibra.AgriculturalPosts.API.csproj
│
├── 📂 frontend/
│   │
│   ├── 📂 src/
│   │   ├── 📂 app/                         # App Router do Next.js 14
│   │   │   ├── 📄 layout.tsx              # Layout principal
│   │   │   ├── 📄 page.tsx                # Página principal
│   │   │   └── 📄 globals.css             # Estilos globais
│   │   │
│   │   ├── 📂 components/                  # Componentes React
│   │   │   ├── 📄 PostCard.tsx            # Card de visualização de post
│   │   │   ├── 📄 CreatePostForm.tsx      # Formulário de criação
│   │   │   └── 📄 PostDetail.tsx          # Modal de detalhes completos
│   │   │
│   │   ├── 📂 lib/                         # Bibliotecas e utilitários
│   │   │   └── 📄 api.ts                  # Cliente HTTP (axios)
│   │   │
│   │   ├── 📂 store/                       # Gerenciamento de estado
│   │   │   └── 📄 usePostStore.ts         # Store Zustand
│   │   │
│   │   └── 📂 types/                       # Definições TypeScript
│   │       └── 📄 post.ts                 # Tipos e interfaces
│   │
│   ├── 📂 public/                          # Arquivos estáticos
│   │
│   ├── 📄 package.json                     # Dependências NPM
│   ├── 📄 tsconfig.json                    # Configuração TypeScript
│   ├── 📄 tailwind.config.ts              # Configuração Tailwind
│   ├── 📄 next.config.js                   # Configuração Next.js
│   ├── 📄 postcss.config.js               # Configuração PostCSS
│   ├── 📄 Dockerfile                       # Docker para frontend
│   └── 📄 .env.local.example              # Exemplo de variáveis de ambiente
│
├── 📄 docker-compose.yml                   # Orquestração de containers
├── 📄 .env.example                         # Exemplo de variáveis de ambiente
├── 📄 .gitignore                          # Arquivos ignorados pelo Git
├── 📄 README.md                           # Documentação principal
├── 📄 AZURE_DEPLOYMENT.md                 # Guia de deploy no Azure
└── 📄 FOLDER_STRUCTURE.md                 # Este arquivo

```

## 📝 Descrição das Camadas

### Backend - Clean Architecture

#### 1. Domain Layer (Núcleo)
- **Propósito**: Contém as regras de negócio e entidades do domínio
- **Dependências**: Nenhuma (camada mais interna)
- **Responsabilidades**:
  - Definir entidades e value objects
  - Definir interfaces de repositórios
  - Conter lógica de negócio pura

#### 2. Application Layer
- **Propósito**: Casos de uso e orquestração
- **Dependências**: Domain Layer
- **Responsabilidades**:
  - Implementar Commands (CQRS)
  - Implementar Queries (CQRS)
  - Definir DTOs para comunicação
  - Validação de entrada

#### 3. Infrastructure Layer
- **Propósito**: Implementações técnicas
- **Dependências**: Domain Layer, Application Layer
- **Responsabilidades**:
  - Implementar repositórios
  - Configurar Entity Framework
  - Implementar serviços externos (OpenAI)
  - Gerenciar conexões com banco de dados

#### 4. API Layer (Presentation)
- **Propósito**: Interface HTTP/REST
- **Dependências**: Todas as camadas
- **Responsabilidades**:
  - Expor endpoints REST
  - Configurar injeção de dependências
  - Middleware e filters
  - Documentação Swagger

### Frontend - Next.js 14

#### 1. App Directory
- **Propósito**: Páginas e layouts (App Router)
- **Estrutura**:
  - `layout.tsx`: Layout compartilhado
  - `page.tsx`: Páginas da aplicação
  - `globals.css`: Estilos globais

#### 2. Components
- **Propósito**: Componentes React reutilizáveis
- **Padrão**: Um componente por arquivo
- **Convenção**: PascalCase para nomes

#### 3. Lib
- **Propósito**: Utilitários e configurações
- **Conteúdo**: 
  - Cliente API
  - Helpers
  - Constantes

#### 4. Store
- **Propósito**: Gerenciamento de estado global
- **Ferramenta**: Zustand
- **Padrão**: Um store por domínio

#### 5. Types
- **Propósito**: Definições TypeScript
- **Conteúdo**:
  - Interfaces
  - Types
  - Enums

## 🔧 Como Criar Novos Recursos

### Adicionar uma Nova Entidade

1. **Domain/Entities/NomeEntidade.cs**
   ```csharp
   public class NomeEntidade
   {
       // Propriedades e lógica de negócio
   }
   ```

2. **Domain/Interfaces/INomeEntidadeRepository.cs**
   ```csharp
   public interface INomeEntidadeRepository
   {
       // Métodos do repositório
   }
   ```

3. **Infrastructure/Repositories/NomeEntidadeRepository.cs**
   ```csharp
   public class NomeEntidadeRepository : INomeEntidadeRepository
   {
       // Implementação
   }
   ```

4. **API/Controllers/NomeEntidadesController.cs**
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class NomeEntidadesController : ControllerBase
   {
       // Endpoints
   }
   ```

### Adicionar um Novo Componente Frontend

1. **components/NomeComponente.tsx**
   ```typescript
   export default function NomeComponente() {
       // Implementação
   }
   ```

2. Importar no **page.tsx** ou outro componente
   ```typescript
   import NomeComponente from '@/components/NomeComponente';
   ```

## 🎯 Boas Práticas

### Backend

- ✅ Um arquivo por classe
- ✅ Interfaces no Domain
- ✅ Implementações no Infrastructure
- ✅ DTOs para comunicação externa
- ✅ Async/await em operações I/O
- ✅ Dependency Injection
- ✅ Logging estruturado

### Frontend

- ✅ Componentes pequenos e focados
- ✅ Server Components por padrão
- ✅ Client Components quando necessário
- ✅ Tipos TypeScript para tudo
- ✅ Tailwind para estilos
- ✅ Hooks customizados em arquivos separados
- ✅ Error boundaries

## 📦 Convenções de Nomenclatura

### Backend (C#)
- **Classes**: PascalCase (`PostRepository`)
- **Interfaces**: I + PascalCase (`IPostRepository`)
- **Métodos**: PascalCase (`GetByIdAsync`)
- **Propriedades**: PascalCase (`UserId`)
- **Variáveis**: camelCase (`userId`)
- **Constantes**: PascalCase (`MaxRetryCount`)

### Frontend (TypeScript/React)
- **Componentes**: PascalCase (`PostCard`)
- **Arquivos**: PascalCase (`PostCard.tsx`)
- **Hooks**: camelCase com 'use' (`usePostStore`)
- **Funções**: camelCase (`fetchPosts`)
- **Interfaces**: PascalCase (`Post`, `PostResponse`)
- **Variáveis**: camelCase (`posts`, `loading`)

## 🔍 Navegação Rápida

### Backend
```bash
# Entidade principal
backend/Domain/Entities/Post.cs

# Lógica de aplicação
backend/Application/Commands/CreatePostCommand.cs

# Acesso a dados
backend/Infrastructure/Repositories/PostRepository.cs

# API REST
backend/API/Controllers/PostsController.cs
```

### Frontend
```bash
# Página principal
frontend/src/app/page.tsx

# Componente de post
frontend/src/components/PostCard.tsx

# Gerenciamento de estado
frontend/src/store/usePostStore.ts

# Cliente API
frontend/src/lib/api.ts
```

---

Esta estrutura foi projetada seguindo os princípios de **Clean Architecture**, **SOLID** e **DRY** para máxima manutenibilidade e escalabilidade.
