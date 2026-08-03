# Vehicle Manager

Aplicação para controle de estoque de veículos, desenvolvida como desafio técnico Full Stack. O projeto possui uma API REST em .NET e uma SPA em Vue 3 que permite cadastrar, listar, buscar, editar e excluir veículos.

## Tecnologias

- Backend: .NET 8, ASP.NET Core Web API e Entity Framework Core 8
- Banco de dados: PostgreSQL 16, executado localmente com Docker Compose
- Frontend: Vue 3, Vite, Vue Router e Axios
- Documentação da API: Swagger

## Pré-requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- Node.js 20 ou superior (o projeto foi testado com Node 22)
- Docker e Docker Compose
- Ferramenta do Entity Framework Core 8:

```bash
dotnet tool install --global dotnet-ef --version 8.0.18
```

> Caso a ferramenta já esteja instalada, use `dotnet tool update --global dotnet-ef --version 8.0.18`.

## Como executar

### 1. Subir o banco de dados

Na raiz do projeto, inicie o PostgreSQL:

```bash
docker compose up -d
```

O banco local será criado com estas configurações:

| Item | Valor |
| --- | --- |
| Host | `localhost` |
| Porta | `5432` |
| Banco | `vehiclemanager` |
| Usuário | `postgres` |
| Senha | `postgres` |

Para parar apenas os containers depois do uso:

```bash
docker compose down
```

### 2. Executar o backend

Em outro terminal:

```bash
cd backend/src/VehicleManager.Api
dotnet restore
dotnet ef database update
dotnet run --launch-profile http
```

A API fica disponível em `http://localhost:5012` e o Swagger em [http://localhost:5012/swagger](http://localhost:5012/swagger).

### 3. Executar o frontend

Em um terceiro terminal:

```bash
cd frontend
cp .env.example .env
npm install
npm run dev
```

Abra o endereço exibido pelo Vite — normalmente [http://localhost:5173](http://localhost:5173).

O arquivo `.env` local contém a URL da API:

```env
VITE_API_URL=http://localhost:5012/api
```

Ele não é versionado. O arquivo `frontend/.env.example` serve como modelo para uma instalação nova.

## Endpoints da API

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/api/veiculos` | Lista veículos; aceita `?busca=texto` para pesquisar por marca, modelo ou placa. |
| `GET` | `/api/veiculos/{id}` | Retorna um veículo ou `404` caso não exista. |
| `POST` | `/api/veiculos` | Cria um veículo e retorna `201 Created`. |
| `PUT` | `/api/veiculos/{id}` | Atualiza um veículo ou retorna `404`. |
| `DELETE` | `/api/veiculos/{id}` | Exclui um veículo e retorna `204 No Content`. |

As regras de validação retornam `400 Bad Request`. Uma placa já cadastrada retorna `409 Conflict` com uma mensagem clara.

## Como testar manualmente

1. Abra o frontend e cadastre um veículo com uma placa válida, por exemplo `ABC1D23`.
2. Confira se ele aparece na tabela com preço em R$, quilometragem e status formatados.
3. Pesquise pela marca, modelo e placa.
4. Edite algum dado, como cor, preço ou status, e confirme a alteração.
5. Tente cadastrar a mesma placa para verificar o tratamento de conflito.
6. Tente enviar uma placa inválida, preço zero, quilometragem negativa ou ano inválido para conferir as mensagens de validação.
7. Exclua o veículo e confirme que a ação pede confirmação antes de remover o registro.
8. Com a lista vazia, confira o estado de “Nenhum veículo cadastrado”.

Para validar os builds sem iniciar os servidores:

```bash
dotnet build backend/VehicleManager.slnx
cd frontend && npm run build
```

## Decisões técnicas

- **PostgreSQL com Docker Compose:** facilita a execução local sem exigir uma instalação manual do banco.
- **DTOs separados da entidade:** a entidade do Entity Framework não é exposta pela API. Isso deixa o contrato HTTP explícito e evita que campos internos sejam alterados pelo cliente.
- **Camadas simples:** o controller cuida de HTTP, o service concentra regras de negócio e o `ApplicationDbContext` acessa o banco. A estrutura atende ao escopo sem introduzir complexidade desnecessária.
- **Validação em dois níveis:** Data Annotations validam formato, obrigatoriedade e limites; o service valida regras que dependem de outros campos, como a relação entre anos e os valores dos enums.
- **Índice único de placa:** além da verificação no service para responder `409`, o banco também possui índice único para proteger a integridade dos dados.
- **Guid como chave:** a aplicação gera o `Guid` ao criar a entidade, impedindo que o cliente informe o identificador. Essa escolha facilita a criação do registro antes da persistência.
- **Variável de ambiente no frontend:** a URL da API não fica fixa nos componentes e pode mudar entre ambientes sem alteração de código.

## O que ficou de fora

O foco foi concluir o CRUD e seus fluxos principais com qualidade. Com mais tempo, eu adicionaria:

- Testes unitários para a camada de serviço e testes de integração para os endpoints.
- Paginação e ordenação da listagem no servidor.
- Máscara de placa e formatação monetária durante a digitação no formulário.
- Filtros mais específicos por status e faixa de preço.
- Autenticação e autorização para controlar quem pode alterar o estoque.
- Dockerização da API, além do banco de dados já disponível no `docker-compose.yml`.
- Deploy do frontend, API e banco em ambiente público.
