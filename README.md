# Developer Evaluation Project

`READ CAREFULLY`

## Instructions

**The test below will have up to 7 calendar days to be delivered from the date of receipt of this manual.**

- The code must be versioned in a public Github repository and a link must be sent for evaluation once completed
- Upload this template to your repository and start working from it
- Read the instructions carefully and make sure all requirements are being addressed
- The repository must provide instructions on how to configure, execute and test the project
- Documentation and overall organization will also be taken into consideration

## Use Case

**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

- Sale number
- Date when the sale was made
- Customer
- Total sale amount
- Branch where the sale was made
- Products
- Quantities
- Unit prices
- Discounts
- Total amount for each item
- Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:

- SaleCreated
- SaleModified
- SaleCancelled
- ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

- Purchases above 4 identical items have a 10% discount
- Purchases between 10 and 20 identical items have a 20% discount
- It's not possible to sell above 20 identical items
- Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:

   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview

This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates.

See [Overview](/.doc/overview.md)

## Tech Stack

This section lists the key technologies used in the project, including the backend, testing, frontend, and database components.

See [Tech Stack](/.doc/tech-stack.md)

## Frameworks

This section outlines the frameworks and libraries that are leveraged in the project to enhance development productivity and maintainability.

See [Frameworks](/.doc/frameworks.md)

<!--
## API Structure
This section includes links to the detailed documentation for the different API resources:
- [API General](./docs/general-api.md)
- [Products API](/.doc/products-api.md)
- [Carts API](/.doc/carts-api.md)
- [Users API](/.doc/users-api.md)
- [Auth API](/.doc/auth-api.md)
-->

## Project Structure

This section describes the overall structure and organization of the project files and directories.

See [Project Structure](/.doc/project-structure.md)

## Configuração

### Pré-requisitos

- Docker e Docker Compose instalados
- .NET 8 SDK (para desenvolvimento local)

### Configuração do Banco de Dados

Este projeto utiliza MongoDB como banco de dados. Para rodar o MongoDB e outras ferramentas de desenvolvimento, execute:

```bash
docker-compose up -d
```

Isso irá subir:

- **MongoDB** na porta 27017 (usuário: `admin`, senha: `senhaforte`)

### Configuração do appsettings.json

Certifique-se de que a string de conexão do MongoDB no arquivo `src/api/DeveloperEvaluation.Api/appsettings.json` está correta:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://admin:senhaforte@localhost:27017",
    "DatabaseName": "DeveloperEvaluationDB"
  }
}
```

## Execução

### Executar Localmente

1. Certifique-se de que o MongoDB está rodando (veja seção de Configuração)
2. Navegue até o diretório da API:
   ```bash
   cd src/api/DeveloperEvaluation.Api
   ```
3. Execute o projeto:
   ```bash
   dotnet run
   ```
4. A API estará disponível em:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`
   - Swagger UI: `http://localhost:5000` ou `https://localhost:5001`

### Executar via Docker

1. Build da imagem Docker:
   ```bash
   docker build -t developer-evaluation-api .
   ```
2. Executar o container:
   ```bash
   docker run -p 8080:8080 --network dev_net developer-evaluation-api
   ```
3. A API estará disponível em `http://localhost:8080`

## Testes

### Testes Unitários

Os testes unitários estão localizados em `tests/DeveloperEvaluation.Tests/` e testam a lógica de negócio de forma isolada, incluindo:

- Validações de domínio
- Comandos e queries
- Serviços de aplicação
- Mappers

**Para executar os testes unitários:**

```bash
cd tests/DeveloperEvaluation.Tests
dotnet test
```

### Testes E2E (End-to-End)

Os testes E2E estão em `tests/DeveloperEvaluation.E2E/` e testam a aplicação completa, incluindo:

- Integração com banco de dados
- Endpoints da API
- Fluxos completos de negócio
- Autenticação e autorização

**Para executar os testes E2E:**

```bash
cd tests/DeveloperEvaluation.E2E
dotnet test
```

**Para executar todos os testes:**

```bash
dotnet test
```

> **Nota:** Os testes E2E utilizam TestContainers para criar um ambiente isolado com MongoDB durante a execução dos testes.
> **Nota:** Foram deixa
