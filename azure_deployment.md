# 🔷 Guia de Deploy no Azure

Documentação para deploy da solução na infraestrutura Azure.

## 📋 Recursos Necessários

### Azure Services

1. **Azure SQL Database**
   - Tier: Standard S1 ou superior
   - DTU: 20+
   - Backup automático habilitado

2. **Azure App Service**
   - 2 App Services (Backend API + Frontend)
   - Plan: B1 (Basic) ou superior
   - Runtime: .NET 9 / Node.js 20

3. **Azure OpenAI Service** (alternativa ao OpenAI direto)
   - Model deployment: GPT-4o-mini
   - Region: East US, West Europe ou similar

4. **Azure Container Registry** (opcional)
   - Para armazenar imagens Docker customizadas

5. **Azure Key Vault**
   - Para armazenar secrets (API keys, connection strings)

6. **Application Insights**
   - Monitoramento e telemetria

## 🚀 Passo a Passo do Deploy

### 1. Provisionar Recursos

```bash
# Login no Azure
az login

# Criar Resource Group
az group create \
  --name rg-cibra-agricultural \
  --location brazilsouth

# Criar Azure SQL Database
az sql server create \
  --name sql-cibra-agricultural \
  --resource-group rg-cibra-agricultural \
  --location brazilsouth \
  --admin-user sqladmin \
  --admin-password YourStrongPassword123!

az sql db create \
  --resource-group rg-cibra-agricultural \
  --server sql-cibra-agricultural \
  --name CibraAgriculturalPosts \
  --service-objective S1

# Criar App Service Plan
az appservice plan create \
  --name plan-cibra-agricultural \
  --resource-group rg-cibra-agricultural \
  --location brazilsouth \
  --sku B1 \
  --is-linux

# Criar App Service para Backend
az webapp create \
  --name app-cibra-agricultural-api \
  --resource-group rg-cibra-agricultural \
  --plan plan-cibra-agricultural \
  --runtime "DOTNETCORE:9.0"

# Criar App Service para Frontend
az webapp create \
  --name app-cibra-agricultural-web \
  --resource-group rg-cibra-agricultural \
  --plan plan-cibra-agricultural \
  --runtime "NODE:20-lts"
```

### 2. Configurar Azure OpenAI (Opcional)

```bash
# Criar Azure OpenAI Service
az cognitiveservices account create \
  --name openai-cibra-agricultural \
  --resource-group rg-cibra-agricultural \
  --location eastus \
  --kind OpenAI \
  --sku S0

# Deploy do modelo GPT-4o-mini
az cognitiveservices account deployment create \
  --name openai-cibra-agricultural \
  --resource-group rg-cibra-agricultural \
  --deployment-name gpt-4o-mini \
  --model-name gpt-4o-mini \
  --model-version "2024-07-18" \
  --model-format OpenAI \
  --sku-capacity 1 \
  --sku-name "Standard"
```

### 3. Configurar Key Vault

```bash
# Criar Key Vault
az keyvault create \
  --name kv-cibra-agricultural \
  --resource-group rg-cibra-agricultural \
  --location brazilsouth

# Adicionar secrets
az keyvault secret set \
  --vault-name kv-cibra-agricultural \
  --name "SqlConnectionString" \
  --value "Server=tcp:sql-cibra-agricultural.database.windows.net,1433;Initial Catalog=CibraAgriculturalPosts;Persist Security Info=False;User ID=sqladmin;Password=YourStrongPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

az keyvault secret set \
  --vault-name kv-cibra-agricultural \
  --name "OpenAIApiKey" \
  --value "your-openai-api-key"
```

### 4. Deploy do Backend

```bash
cd backend/API

# Build e publish
dotnet publish -c Release -o ./publish

# Criar arquivo ZIP
cd publish
zip -r ../deploy.zip .
cd ..

# Deploy para App Service
az webapp deployment source config-zip \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api \
  --src deploy.zip

# Configurar variáveis de ambiente
az webapp config appsettings set \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection="@Microsoft.KeyVault(SecretUri=https://kv-cibra-agricultural.vault.azure.net/secrets/SqlConnectionString/)" \
    OpenAI__ApiKey="@Microsoft.KeyVault(SecretUri=https://kv-cibra-agricultural.vault.azure.net/secrets/OpenAIApiKey/)"
```

### 5. Deploy do Frontend

```bash
cd frontend

# Build
npm run build

# Deploy para App Service
az webapp deployment source config-zip \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-web \
  --src .next.zip

# Configurar variáveis de ambiente
az webapp config appsettings set \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-web \
  --settings \
    NODE_ENV=production \
    NEXT_PUBLIC_API_URL="https://app-cibra-agricultural-api.azurewebsites.net"
```

## 🔧 Configurações Adicionais

### Application Insights

```bash
# Criar Application Insights
az monitor app-insights component create \
  --app insights-cibra-agricultural \
  --location brazilsouth \
  --resource-group rg-cibra-agricultural

# Obter Instrumentation Key
INSTRUMENTATION_KEY=$(az monitor app-insights component show \
  --app insights-cibra-agricultural \
  --resource-group rg-cibra-agricultural \
  --query instrumentationKey -o tsv)

# Adicionar ao Backend
az webapp config appsettings set \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api \
  --settings \
    APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$INSTRUMENTATION_KEY"
```

### CORS

```bash
# Configurar CORS no Backend
az webapp cors add \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api \
  --allowed-origins "https://app-cibra-agricultural-web.azurewebsites.net"
```

### SSL/TLS

```bash
# Habilitar HTTPS only
az webapp update \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api \
  --https-only true

az webapp update \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-web \
  --https-only true
```

## 🔐 Managed Identity

Para melhor segurança, configure Managed Identity:

```bash
# Habilitar System Managed Identity no App Service
az webapp identity assign \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api

# Obter Object ID
OBJECT_ID=$(az webapp identity show \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api \
  --query principalId -o tsv)

# Dar permissões ao Key Vault
az keyvault set-policy \
  --name kv-cibra-agricultural \
  --object-id $OBJECT_ID \
  --secret-permissions get list
```

## 📊 Monitoramento

### Queries Úteis do Application Insights

```kusto
// Requisições com erro
requests
| where success == false
| summarize count() by resultCode, name
| order by count_ desc

// Tempo médio de resposta
requests
| summarize avg(duration) by bin(timestamp, 5m)
| render timechart

// Chamadas à OpenAI API
dependencies
| where name contains "openai"
| summarize count(), avg(duration) by resultCode
```

## 💰 Estimativa de Custos (Mensal)

| Serviço | Tier | Custo Estimado |
|---------|------|----------------|
| Azure SQL Database | S1 | ~R$ 100 |
| App Service Plan | B1 | ~R$ 80 |
| Azure OpenAI | Pay-as-go | ~R$ 50-200 |
| Application Insights | Basic | ~R$ 30 |
| Key Vault | Standard | ~R$ 5 |
| **Total** | | **~R$ 265-415** |

## 🔄 CI/CD com GitHub Actions

```yaml
# .github/workflows/deploy.yml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  deploy-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      
      - name: Build and publish
        run: |
          cd backend/API
          dotnet publish -c Release -o publish
      
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: app-cibra-agricultural-api
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: backend/API/publish

  deploy-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '20'
      
      - name: Build
        run: |
          cd frontend
          npm install
          npm run build
      
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: app-cibra-agricultural-web
          publish-profile: ${{ secrets.AZURE_FRONTEND_PUBLISH_PROFILE }}
          package: frontend/.next
```

## 🧪 Testes no Ambiente Azure

```bash
# Health Check Backend
curl https://app-cibra-agricultural-api.azurewebsites.net/health

# Swagger
open https://app-cibra-agricultural-api.azurewebsites.net/swagger

# Frontend
open https://app-cibra-agricultural-web.azurewebsites.net
```

## 📝 Checklist de Deploy

- [ ] Todos os recursos Azure provisionados
- [ ] Connection strings no Key Vault
- [ ] Managed Identity configurada
- [ ] Application Insights habilitado
- [ ] CORS configurado corretamente
- [ ] HTTPS only habilitado
- [ ] Backup automático do SQL configurado
- [ ] Migrations aplicadas no banco
- [ ] Testes de smoke realizados
- [ ] Monitoramento configurado
- [ ] Alertas criados

## 🆘 Troubleshooting Azure

### Logs do App Service

```bash
# Stream de logs
az webapp log tail \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api

# Download de logs
az webapp log download \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api \
  --log-file logs.zip
```

### Restart de Serviços

```bash
az webapp restart \
  --resource-group rg-cibra-agricultural \
  --name app-cibra-agricultural-api
```

---

Para mais informações, consulte a [documentação oficial do Azure](https://docs.microsoft.com/azure).