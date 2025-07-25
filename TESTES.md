# 🧪 Como Executar os Testes

## Executando Todos os Testes

```bash
# Na raiz do projeto
dotnet test

# Com mais detalhes
dotnet test --verbosity normal

# Com cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

## Executando Testes Específicos

```bash
# Somente testes da entidade Sale
dotnet test --filter "FullyQualifiedName~SaleTests"

# Somente testes de Command Handlers
dotnet test --filter "FullyQualifiedName~CommandHandlerTests"

# Teste específico por nome
dotnet test --filter "Method=Sale_Should_Apply_10_Percent_Discount_For_4_To_9_Items"
```

## Executando em Watch Mode

```bash
# Para desenvolvimento - executa testes automaticamente quando arquivos mudam
dotnet watch test
```

## Estrutura dos Testes

### 📁 `DeveloperEvaluation.Tests/Domain/Entities/`

- **SaleTests.cs**: Testa regras de negócio da entidade `Sale`
  - ✅ Cálculo de total de venda
  - ✅ Aplicação de descontos (10% para 4-9 itens, 20% para 10-20 itens)
  - ✅ Validação de limite máximo (20 itens)
  - ✅ Cancelamento de vendas
  - ✅ Validações de negócio

### 📁 `DeveloperEvaluation.Tests/Application/Commands/`

- **CreateSaleCommandHandlerTests.cs**: Testa handlers de comando
  - ✅ Criação de vendas via CQRS
  - ✅ Aplicação correta de regras de negócio
  - ✅ Publicação de eventos de domínio

## Tecnologias Utilizadas nos Testes

- **xUnit**: Framework de testes principal
- **FluentAssertions**: Para asserções mais legíveis
- **NSubstitute**: Para criação de mocks e stubs
- **AutoMapper**: Para mapeamento de objetos
- **MediatR**: Para testes de CQRS

## Regras de Negócio Testadas

### 🎯 Descontos por Quantidade

- **Menos de 4 itens**: Sem desconto
- **4 a 9 itens**: 10% de desconto
- **10 a 20 itens**: 20% de desconto
- **Mais de 20 itens**: ❌ Não permitido

### 🚫 Validações

- Limite máximo de 20 itens por produto
- Não é possível adicionar itens a vendas canceladas
- Não é possível cancelar vendas já canceladas

## Executando com MongoDB

Para executar os testes integrados com MongoDB, certifique-se de que:

1. **MongoDB está rodando**: `mongodb://admin:senhaforte@localhost:27017`
2. **String de conexão está configurada** no `appsettings.json`
3. **Projeto compilado**: `dotnet build`

```bash
# Verificar se MongoDB está acessível
dotnet run --project DeveloperEvaluation.Api
```

## Cobertura de Código

Para gerar relatório de cobertura:

```bash
# Instalar ferramenta de relatório
dotnet tool install -g dotnet-reportgenerator-globaltool

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório HTML
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

## 🎉 Status dos Testes

**✅ 9/9 Testes Passando**

- ✅ Regras de negócio de descontos
- ✅ Validações de quantidade
- ✅ Cancelamento de vendas
- ✅ CQRS Command Handlers
- ✅ Eventos de domínio
- ✅ Mapeamento de DTOs

---

**Dica**: Use `dotnet watch test` durante o desenvolvimento para feedback instantâneo! 🚀
