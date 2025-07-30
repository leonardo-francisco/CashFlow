# CashFlow

**CashFlow** é uma aplicação para gerenciar o fluxo de caixa diário de comerciantes, registrando lançamentos de créditos e débitos e gerando um relatório consolidado de saldo diário. Desenvolvida em .NET Core e MongoDB, a aplicação segue princípios de arquitetura limpa e boas práticas para garantir escalabilidade, segurança e resiliência.

## Funcionalidades

- **Gerenciamento de Transações**: Adiciona lançamentos de crédito e débito com informações como data, valor e descrição.
- **Consolidação Diária de Saldo**: Gera um relatório consolidado do saldo para cada dia, incluindo o total de débitos e créditos.
- **OpenAPI Scalar**: Interface para acesso e gerenciamento de transações e relatórios diários.
- **Escalabilidade e Resiliência**: Arquitetura preparada para crescimento e recuperação de falhas.

## Estrutura do Projeto
CashFlow/ 
  ├── CashFlow.Domain/ # Camada de domínio (entidades, serviços, interfaces de domínio) 
  ├── CashFlow.Application/ # Camada de aplicação (serviços, DTOs, CQRS) 
  ├── CashFlow.Infrastructure/ # Camada de infraestrutura (repositórios, configuração de banco MongoDB, redis, RabbitMq) 
  ├── CashFlow.Presentation/ # Camada de apresentação (API) 
  ├── CashFlow.Tests/ # Testes automatizados para cada camada 
  └── README.md # Documentação e instruções de uso

## Requisitos

- **.NET 9 SDK**
- **MongoDB** (executando localmente ou usando MongoDB Atlas)
- **Scalar UI para API**
- **RabbitMQ**
- **Redis**
- **SOLID**
- **DDD**
- **Clean Code**
- **JWT**

## Configuração

### 1. Clonar o Repositório

```bash
git clone https://github.com/leonardo-francisco/CashFlow.git
cd CashFlow
```

### 2. Clonar o MongoDB
- **Localmente: Inicie o MongoDB em sua máquina**
- **Docker: Caso prefira usar Docker, você pode iniciar o MongoDB com o comando:
```bash
docker run -d -p 27017:27017 --name CashFlow mongo
```

### 3. Configuração do Arquivo appsettings.json
- **No diretório CashFlow.Presentation, crie um arquivo appsettings.json e configure a string de conexão do MongoDB**
```bash
  "MongoSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "CashFlowApp"
},
"AdminCredentials": {
    "Username": "admin@cashflow.com.br",
    "Password": "123456"
},
"Key": {
    "Secret": "fedaf7d8863b48e197b9287d492b708e",
    "Issuer": "meuSistema",
    "Audience": "meuSistemaUsuario"
},
"ConnectionStrings": {
    "Redis": "localhost:6379"
},
"RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "Exchange": "transactions.exchange"
},
"Polly": {
    "RetryCount": 3,
    "CircuitBreakerAllowedFailures": 5,
    "CircuitBreakerDuration": 30
},
"Logging": {
    "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
    }
},
"AllowedHosts": "*"
```
- **Caso opte para executar via Docker o  arquivo appsettings.json deve ser diferente a string de conexão**
```bash
   "MongoSettings": {
     "ConnectionString": "mongodb://mongo:27017",
     "DatabaseName": "CashFlowApp"
 }
```

## Executando a Aplicação

### 1. Restaurar Dependências
- **No diretório raiz do projeto, execute:
```bash
dotnet restore
```

### 2. Rodar a Aplicação
- **Para iniciar a API, execute o seguinte comando:
```bash
dotnet run --project CashFlow.API
```
- **A aplicação será iniciada na rota Login deve ser usado as credenciais do AdminCredential para autenticar e testar os demais endpoints

### 3. Testando a Aplicação
- **Para rodar os testes:
```bash
dotnet test
```
- **Isso executará os testes em CashFlow.Test e exibirá os resultados no terminal

## Endpoints Principais

### Autenticação
- `POST /authentication/login`: Gera token JWT

### Transações
- `POST /transaction`: Adiciona uma nova transação (crédito ou débito).
- `GET /transaction/{date}`: Recupera uma transação pela data.
- `GET /dailyBalance/{date}`: Recupera um balanço consolidado pela data.

## Arquitetura e Escalabilidade

### Tecnologias e Padrões

- CQRS com MediatR
- Mensageria com RabbitMQ
- Cache distribuído com Redis
- Resiliência com Polly (retry + circuit breaker)
- JWT para autenticação
- OpenAPI Scalar UI

### Componentes

| Componente              | Responsabilidade                                   |
|-------------------------|----------------------------------------------------|
| Presentation            | API REST, autenticação e roteamento                |
| Application             | Orquestração via CQRS, DTOs e comandos             |
| Domain                  | Entidades e lógica de negócio                      |
| Infrastructure          | Repositórios, MongoDB, Redis e mensageria          |

## Escalabilidade e Resiliência

- Escalável horizontalmente com múltiplas instâncias da API
- Cache Redis compartilhado entre instâncias
- RabbitMQ desacopla gravação e leitura (tolerância a falhas)
- Circuit Breaker e Retry com Polly garantem resiliência
- Funciona mesmo se Redis ou RabbitMQ estiverem offline

## Requisitos Não Funcionais Atendidos

| Requisito                           | Status |
|------------------------------------|--------|
| Serviços desacoplados              | ✅     |
| Cache distribuído                  | ✅     |
| Alta disponibilidade               | ✅     |
| Tolerância a falhas                | ✅     |
| Performance para 50 req/s          | ✅     |

## Melhorias Futuras

- Separação física das APIs em microsserviços reais
- API Gateway e autenticação centralizada
- Monitoramento com Prometheus + Grafana
- Dashboard administrativo