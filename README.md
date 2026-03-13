# Recadastramento API

API REST em .NET 8 para manipular `recadastra.recad_persona` (PostgreSQL), com:
- Upsert em lote por CPF
- Consulta por CPF e listagem com filtros
- Exportacao em JSON, CSV e XLSX
- Autenticacao por prefeitura com `X-Client-Id` + `X-Api-Key`
- Rota por cidade: `/{cidade}/personas`

## Requisitos

- .NET SDK 8
- PostgreSQL com schema/tabelas do arquivo `Table DDLs.txt`

## Subir em outro PC

1. Clonar/copiar o projeto.
2. Entrar na raiz:

```bash
cd recadastramentoAPI
```

3. Restaurar dependencias:

```bash
dotnet restore
```

4. Ajustar configuracoes em `RecadastramentoApi/appsettings.json`:
- `Database:ConnectionString`
- `ApiSecurity:Clients` (um cliente por prefeitura/cidade)

Exemplo:

```json
"ApiSecurity": {
  "Clients": [
    {
      "ClientId": "prefeitura-saoluis",
      "ApiKeyHash": "<base64-do-hash>",
      "ApiKeySalt": "<base64-do-salt>",
      "Iterations": 120000,
      "City": "saoluis"
    }
  ]
}
```

5. Build:

```bash
dotnet build
```

6. Executar:

```bash
dotnet run --project RecadastramentoApi/RecadastramentoApi.csproj
```

7. Swagger:
- `http://localhost:5090/swagger` (porta padrao de desenvolvimento)

## Como gerar `ApiKeyHash` e `ApiKeySalt`

Exemplo com Python (PBKDF2 SHA-256):

```bash
python3 - <<'PY'
import os, base64, hashlib
api_key = b"SUA_CHAVE_FORTE_AQUI"
salt = os.urandom(16)
iterations = 120000
hash_bytes = hashlib.pbkdf2_hmac("sha256", api_key, salt, iterations, dklen=32)
print("ApiKeySalt =", base64.b64encode(salt).decode())
print("ApiKeyHash =", base64.b64encode(hash_bytes).decode())
print("Iterations =", iterations)
PY
```

Use os valores gerados no `appsettings.json`.

## Modelo de seguranca (prefeitura = cliente)

- Cada prefeitura eh um cliente unico.
- Cada cliente tem uma unica cidade em `ApiSecurity:Clients[n]:City`.
- A requisicao so passa se:
  - `X-Client-Id` e `X-Api-Key` forem validos
  - `/{cidade}` da URL for igual a `City` do cliente autenticado

## Headers obrigatorios

- `X-Client-Id: <id-da-prefeitura>`
- `X-Api-Key: <chave-secreta-da-prefeitura>`

## Rotas principais

- `POST /{cidade}/personas`
- `GET /{cidade}/personas/{cpf}`
- `GET /{cidade}/personas?cpf=...&nome=...&municipio_id=...&uf=...&fkrecadastramento=...&recad_status=...`
- `GET /{cidade}/personas/export?format=json|csv|xlsx`

Nos GETs de persona (`/{cpf}` e listagem), a resposta retorna a persona com os blocos relacionados:
- `manutencoes`
- `anexos`
- `dependentes`
- `conjuges`

## POST com dependentes e conjuges

No mesmo `POST /{cidade}/personas`, voce pode enviar:
- `manutencoes`: grava em `recadastra.recad_manutencao` (funcional da persona)
- `anexos`: grava em `recadastra.recad_anexo`
- `dependentes`: grava em `recadastra.recad_dependente`
- `conjuges`: grava em `recadastra.recad_conjuge`

Regra de upsert dos vinculos:
- Manutencao: se encontrar o mesmo registro para a persona (por `id` ou chave natural), faz `UPDATE`, senao `INSERT`.
- Anexo: se existir o par `(fkpersona, fkanexotipo)` faz `UPDATE`, senao `INSERT`.
- Dependente: se existir o par `(fkresponsavel, fkdependente)` faz `UPDATE`, senao `INSERT`.
- Conjuge: se existir o par `(fkpersona, fkconjuge)` faz `UPDATE`, senao `INSERT`.

Cada item de dependente/conjuge pode referenciar a pessoa vinculada por:
- `fk_dependente` / `fk_conjuge` (id da persona), ou
- `cpf_dependente` / `cpf_conjuge` (a API resolve para `id`).

Campos esperados em `anexos`:
- `fk_anexo_tipo` (obrigatorio)
- `obrigatorio`, `emissao_data`, `validade_data`, `cadastro_fkusuario`, `cadastro_data`, `url`, `observacao` (opcionais)
- `arquivo` (opcional, Base64 no JSON; mapeado para `bytea`)

## Postman

Collection pronta:
- `RecadastramentoApi/postman/RecadastramentoApi.postman_collection.json`

No Postman, ajuste as variaveis:
- `baseUrl`
- `cidade`
- `clientId`
- `apiKey`

## Comandos uteis

```bash
dotnet restore
dotnet build
dotnet run --project RecadastramentoApi/RecadastramentoApi.csproj
```
