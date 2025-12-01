# MunicipiosAPI

A MunicipiosAPI é uma API REST desenvolvida em .NET 10 para consulta de municípios brasileiros a partir de dois providers externos: BrasilAPI e IBGE. A API implementa paginação, cache, tratamento global de exceções, testes automatizados, documentação via Swagger e está preparada para execução local ou em ambiente conteinerizado.

Esta API faz parte de uma solução Fullstack composta também pela aplicação frontend MunicipiosAPP (Angular), que consome este serviço.

## 1. Objetivo da API

Fornecer um endpoint para listar municípios pertencentes a uma Unidade Federativa (UF), com possibilidade de alternância entre dois providers oficiais, seleção por variável de ambiente, resposta paginada e desempenho otimizado com uso de cache.

## 2. Arquitetura da Solução

A API é organizada em quatro camadas principais:

### Domain
Contém modelos, interfaces e contratos utilizados pelas demais camadas. Mantém a base conceitual da aplicação, incluindo a entidade de resposta `MunicipioResponse`, o tipo `MunicipiosProviderType` e a abstração `IProviderMunicipios`.

### Providers
Implementa a comunicação externa com BrasilAPI e IBGE. Cada provider transforma a resposta de sua respectiva API para o modelo interno da aplicação. A seleção do provider ativo ocorre via variável de ambiente e é resolvida via injeção de dependência.

### Service
Centraliza as regras de negócio da aplicação. Esta camada recebe a UF solicitada pelo usuário, aciona o provider configurado, aplica cache para otimização de desempenho, realiza paginação, validações e monta o objeto `PagedResult<T>` retornado ao controller. A camada de serviço é totalmente isolada da apresentação (API Web).

### API (Apresentação)
Responsável por expor os endpoints HTTP, configurar Swagger, mapear rotas, aplicar middleware de tratamento global de exceções e resolver dependências. O controller é enxuto e delega todo o fluxo ao service.

## 3. Tecnologias Utilizadas

- .NET 10
- ASP.NET Core Web API
- HttpClient
- IMemoryCache
- xUnit e Moq
- Swagger / Swashbuckle
- Docker
- GitHub Actions (CI)

## 4. Endpoint Principal

### Listar municípios por UF

```
GET /Municipios/{uf}?page={page}&pageSize={pageSize}
```

### Exemplo de resposta:

```json
{
  "page": 1,
  "pageSize": 10,
  "totalItems": 497,
  "totalPages": 50,
  "items": [
    {
      "name": "Porto Alegre",
      "ibgeCode": "4314902"
    }
  ]
}
```

## 5. Configuração via Variáveis de Ambiente

| Variável | Função | Valores permitidos |
|---------|--------|--------------------|
| PROVIDER_MUNICIPIOS | Define qual provider será utilizado | BRASIL_API ou IBGE |

## 6. Execução da API

### Local

```
dotnet restore
dotnet build
dotnet run
```

Swagger: http://localhost:5119/swagger/index.html

## 7. Docker

```
docker build -t municipiosapi .
docker run -p 5119:8080 -e PROVIDER_MUNICIPIOS=IBGE municipiosapi
```

## 8. Testes

```
dotnet test
```

## 9. Estrutura

```
/MunicipiosAPI
/MunicipiosAPI.Domain
/MunicipiosAPI.Providers
/MunicipiosAPI.Service
/MunicipiosAPI.Tests