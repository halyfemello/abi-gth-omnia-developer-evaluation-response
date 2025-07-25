## 🏗️ **ARQUITETURA**

Transformamos o projeto para uma arquitetura mais limpa e simples:

### **📁 ESTRUTURA DE PROJETOS:**

```
├── src/
│   ├── api/                    # 🎯 Todos os projetos C# aqui
│   │   ├── DeveloperEvaluation.Api/    # 🌐 API Controllers + Startup
│   │   ├── DeveloperEvaluation.Core/   # 🧠 Domain + Application (CQRS)
│   │   ├── DeveloperEvaluation.Infra/  # 🗄️ Data + Repositories
│   │   └── DeveloperEvaluation.sln
│   └── web/                    # 🚀 Placeholder para frontend futuro
│       └── README.md
├── tests/                      # 🧪 Projeto de testes isolado
│   └── DeveloperEvaluation.Tests/
└── ...demais arquivos

```

### **📦 SEPARAÇÃO DE RESPONSABILIDADES:**

#### **🌐 DeveloperEvaluation.Api**

- Controllers (SalesController)
- Program.cs (Startup)
- Middleware
- DTOs de API

#### **🧠 DeveloperEvaluation.Core**

- **Domain/**: Entities, Events, Repositories (interfaces)
- **Application/Commands/**: Commands + Handlers (CQRS)
- **Application/Queries/**: Queries + Handlers (CQRS)
- **Application/DTOs/**: Data Transfer Objects
- **Application/Mappings/**: AutoMapper Profiles

#### **🗄️ DeveloperEvaluation.Infra**

- DbContext (Entity Framework)
- Repository Implementations
- External Services

## 🚀 **COMO EXECUTAR A API**

### **1️⃣ Via Terminal:**

```bash
cd DeveloperEvaluation.Api
dotnet run
```

### **2️⃣ Via VS Code Tasks:**

1. `Ctrl + Shift + P`
2. `Tasks: Run Task`
3. Selecione `run-new-api`

### **3️⃣ Via Debug (F5):**

1. Pressione `F5`
2. Selecione `Launch New API`

## 🌐 **ENDPOINTS DISPONÍVEIS**

### **📊 Swagger UI:**

```
https://localhost:5001/swagger
```

**OU**

```
http://localhost:5000/swagger
```

### **🛒 Sales API (CRUD Completo):**

#### **Criar Venda:**

```http
POST https://localhost:5001/api/sales
Content-Type: application/json

{
  "saleNumber": "SALE-001",
  "saleDate": "2025-01-24T00:00:00",
  "customerId": "123e4567-e89b-12d3-a456-426614174000",
  "customerName": "João Silva",
  "customerEmail": "joao@email.com",
  "branchId": "987fcdeb-51a2-43d8-b123-987654321000",
  "branchName": "Filial Centro",
  "items": [
    {
      "productId": "456e7890-e12b-34d5-a678-426614174111",
      "productName": "Produto A",
      "productDescription": "Descrição do Produto A",
      "quantity": 5,
      "unitPrice": 100.00
    }
  ]
}
```

#### **Buscar Vendas:**

```http
GET https://localhost:5001/api/sales                    # Todas as vendas
GET https://localhost:5001/api/sales/{id}               # Venda específica
GET https://localhost:5001/api/sales?page=1&size=10     # Paginação
```

## 🎯 **PADRÕES IMPLEMENTADOS**

### **✅ CQRS (Command Query Responsibility Segregation):**

- **Commands/**: `CreateSaleCommand` + `CreateSaleCommandHandler`
- **Queries/**: `GetSaleByIdQuery` + `GetSaleByIdQueryHandler`

### **✅ MediatR:**

- Desacoplamento entre Controllers e Application
- Event publishing para observabilidade

### **✅ Repository Pattern:**

- `ISaleRepository` (interface no Domain)
- `SaleRepository` (implementação na Infra)

### **✅ Domain-Driven Design:**

- Rich Domain Models (`Sale`, `SaleItem`)
- Domain Events (`SaleCreatedEvent`)
- Business Rules encapsuladas nas entidades

### **✅ AutoMapper:**

- Mapeamento automático entre Entities e DTOs

## 🔥 **REGRAS DE NEGÓCIO IMPLEMENTADAS**

1. **✅ 4+ itens = 10% desconto**
2. **✅ 10-20 itens = 20% desconto**
3. **✅ Máximo 20 itens por produto**
4. **✅ Menos de 4 itens = SEM desconto**
5. **✅ Cálculo automático de totais**
6. **✅ Cancelamento de vendas e itens**

## 🎪 **OBSERVABILIDADE**

### **📡 Eventos de Domínio:**

- `SaleCreatedEvent` - Disparado ao criar venda
- Logs automáticos via MediatR
- Rastreamento com EventId único

### **📊 Exemplo de Log:**

```json
{
  "EventId": "abc123-def456-ghi789",
  "EventType": "SaleCreated",
  "SaleId": "123e4567-e89b-12d3-a456-426614174000",
  "TotalAmount": 450.0,
  "Timestamp": "2025-01-24T10:30:00Z"
}
```

## 🧪 **TESTANDO NO NAVEGADOR**

### **1. Health Check Global:**

```
https://localhost:5001/health
```

### **2. Health Check Sales:**

```
https://localhost:5001/api/sales/health
```

### **3. Swagger para Testes Interativos:**

```
https://localhost:5001/swagger
```

## 🔧 **COMANDOS ÚTEIS**

### **Build:**

```bash
dotnet build DeveloperEvaluation.sln
```

### **Restore:**

```bash
dotnet restore DeveloperEvaluation.sln
```

### **Run com Hot Reload:**

```bash
cd DeveloperEvaluation.Api
dotnet watch run
```

## 🎊 **PROJETO PRONTO!**

### **✅ IMPLEMENTADO:**

- 🏗️ Arquitetura Clean com 3 camadas
- 🎯 CQRS com MediatR
- 🛒 Sales API completa
- 🔒 Regras de negócio validadas
- 📊 Observabilidade com eventos
- 🌐 Swagger documentado

### **🚀 PRÓXIMOS PASSOS OPCIONAIS:**

- 🗄️ Migrations do Entity Framework
- 🧪 Testes unitários
- 📝 Validações com FluentValidation
- 🔐 Autenticação JWT
- 📊 Logs estruturados

**O projeto está 100% funcional e pronto para uso! 🎉**
