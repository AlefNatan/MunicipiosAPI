# MunicipiosAPI

A **MunicipiosAPI** é uma API REST desenvolvida em **.NET 10** para consulta de municípios brasileiros utilizando dois providers oficiais: **BrasilAPI** e **IBGE**.  
O projeto implementa paginação, cache, tratamento global de exceções, testes automatizados, documentação via Swagger e suporte completo para execução local, em produção ou em ambiente conteinerizado.

Esta API integra a solução fullstack composta pela aplicação frontend **MunicipiosAPP**, desenvolvida em Angular.

---

## 1. Objetivo da API

Fornecer um endpoint padronizado para listar municípios de uma UF, permitindo alternância entre dois providers externos configurados via variável de ambiente, além de oferecer:

- Resposta paginada  
- Melhoria de desempenho por cache  
- Arquitetura desacoplada (Domain, Providers, Service, API)  
- Testes automatizados  
- Documentação via Swagger  

---

## 2. Arquitetura da Solução

A aplicação segue uma estrutura em camadas, promovendo organização, isolamento e testabilidade.

### **Domain**
Contém modelos e contratos fundamentais:
- `MunicipioResponse`
- `PagedResult<T>`
- `MunicipiosProviderType`
- Interface `IProviderMunicipios`

### **Providers**
Responsáveis por integrar com os serviços externos:
- **BrasilAPI**
- **IBGE**

Cada provider transforma sua resposta para o modelo interno.  
A escolha do provider é feita pela variável de ambiente `PROVIDER_MUNICIPIOS`.

### **Service**
Centraliza toda a regra de negócio:
- Validação da UF
- Chamadas ao provider configurado
- Aplicação de cache (`IMemoryCache`)
- Paginação
- Retorno do modelo `PagedResult<T>`

### **API (Apresentação)**
- Controllers enxutos
- Swagger configurado
- Middleware de tratamento global de exceções
- Resolução de dependências via DI

---

## 3. Tecnologias Utilizadas

- .NET 10  
- ASP.NET Core Web API  
- HttpClient  
- IMemoryCache  
- xUnit, Moq  
- Swagger / Swashbuckle  
- Docker  
- GitHub Actions (CI/CD)

---

## 4. Endpoint Principal

### **Listar municípios por UF**

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

---

## 5. Configuração via Variáveis de Ambiente

| Variável | Descrição | Valores permitidos |
|---------|-----------|--------------------|
| **PROVIDER_MUNICIPIOS** | Define qual provider será utilizado | `BRASIL_API` ou `IBGE` |

---

## 6. Execução da API

### **Rodando localmente**

```
dotnet restore
dotnet build
dotnet run
```

Swagger disponível em:  
➡️ **http://localhost:5119/swagger/index.html**

---

## 7. Docker

### **Build da imagem**
```
docker build -t municipiosapi .
```

### **Executar o container**
```
docker run -p 5119:8080 -e PROVIDER_MUNICIPIOS=IBGE municipiosapi
```

---

## 8. Testes

```
dotnet test
```

Todos os providers, services e controllers possuem testes unitários e de integração.

---

## 9. Estrutura do Projeto

```
/MunicipiosAPI
/MunicipiosAPI.Domain
/MunicipiosAPI.Providers
/MunicipiosAPI.Service
/MunicipiosAPI.Tests
```

---

## 10. Links Úteis

- **API em Produção:** https://municipioapi1-q77lvcud.b4a.run/swagger/index.html  
- **Aplicação Web (SPA):** https://municipios-app.vercel.app
