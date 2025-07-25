## 🏗️ **ARQUITETURA**

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
    └── DeveloperEvaluation.E2E/
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
