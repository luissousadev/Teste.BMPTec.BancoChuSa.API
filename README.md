🏦 BMPTec Banco ChuSa API

API desenvolvida como parte de um desafio técnico, simulando um módulo de operações bancárias, incluindo:

Cadastro de contas bancárias

Transferências financeiras entre contas

Geração de extrato por período

Autenticação JWT

Versionamento de API (API Versioning)

Validações, testes e boas práticas arquiteturais

Execução em ambiente Docker

🚀 Tecnologias Utilizadas
Stack	Detalhes
.NET 6	Web API
Dapper	Acesso a banco de dados
PostgreSQL	Persistência
JWT Bearer Auth	Autenticação
FluentValidation + FluentAssertions	Validação e testes
API Versioning	Suporte a múltiplas versões
Swagger UI (OpenAPI)	Documentação da API
IMemoryCache	Cache para consumo externo
RestSharp	Consumo de API BrasilAPI
Docker	Deploy e runtime
📁 Estrutura do Projeto (Arquitetura)

O projeto segue uma abordagem em camadas, com separação clara por responsabilidade:

src/
 ├─ BMPTec.BancoChuSa.API
 |   ├─ Controllers
 |   ├─ DTOs
 |   ├─ Configurations
 |   ├─ Validators
 |
 ├─ Application
 |   ├─ Services
 |   ├─ Interfaces
 |
 ├─ Domain
 |   ├─ Entities
 |   ├─ Interfaces
 |
 ├─ Infrastructure
 |   ├─ Persistence (Dapper)
 |   ├─ Migrations / Scripts
 |
 └─ Tests
     ├─ Unit Tests
     ├─ Service Tests

🔐 Autenticação

A API utiliza JWT Bearer Token.

Após fazer login no endpoint /api/v1/auth/login, copie o token retornado e clique em Authorize no Swagger.

Exemplo do header:

Authorization: Bearer eyJh...aZx

🧪 Testes Automatizados

O projeto inclui testes utilizando:

xUnit

FluentAssertions

Moq

Execução:

dotnet test

🌍 Integração Externa

A API consulta feriados nacionais usando:

👉 https://brasilapi.com.br/api/feriados/v1/{ano}

Essa consulta é cacheada por 24h utilizando IMemoryCache para evitar chamadas repetidas e garantir desempenho.

🧠 Regras de Negócio Implementadas
Funcionalidade	Validação
Criar conta	Dados obrigatórios + validação de modelo
Transferência	Só permitido entre contas diferentes e com valor > 0
Dias úteis	Transferências bloqueadas em feriados e fins de semana
Extrato	Filtrado por período definindo origem/destino
🐳 Docker
📌 Build da imagem:
docker build -t bmptec-bancochusa-api .

📌 Execução do container:
docker run -d -p 5000:80 --name bmptec-api bmptec-bancochusa-api

Após subir, acessar:

📍 Swagger: http://localhost:5000/swagger

📌 Endpoints Principais
Método	Endpoint	Descrição
POST	/auth/login	Gera token
POST	/bankaccounts	Cria conta
GET	/bankaccounts/{id}	Consulta conta
POST	/transfers	Realiza transferência
GET	/transfers/statement	Extrato
🎯 Objetivo do Projeto

Este projeto tem como propósito demonstrar:

Conhecimento em arquitetura limpa e boas práticas

Organização modular e extensível

Tratamento de exceções e validações profissionais

Segurança com JWT e documentação via Swagger

Integração com serviços externos e uso eficiente de cache

Containerização com Docker
