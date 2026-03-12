# Documentação da API Personas

Esta documentação descreve os requisitos para geração de uma **API REST
em .NET 8** que manipula a tabela **recadastra.recad_persona** em
PostgreSQL.

A API deve receber, gravar, consultar e exportar **TODOS os campos da
tabela**, sem omitir nenhum.

------------------------------------------------------------------------

# Stack obrigatória

-   .NET 8
-   ASP.NET Core Web API
-   Dapper
-   Npgsql
-   CsvHelper
-   ClosedXML
-   Swagger

------------------------------------------------------------------------

# Estrutura do projeto

    Controllers/
    Services/
    Repositories/
    Models/
    DTO/
    Export/
    Database/
    Validators/

------------------------------------------------------------------------

# Model da Persona

Criar um model **Persona** contendo **todos os campos da tabela
recadastra.recad_persona**.

Mapeamento de tipos:

  PostgreSQL   C#
  ------------ ------------------------------
  bigint       long
  integer      int
  smallint     short
  varchar      string
  char         string
  text         string
  date         DateTime?
  timestamp    DateTime?
  boolean      bool
  hstore       Dictionary\<string,string\>?

Campos nullable devem ser nullable no model.

------------------------------------------------------------------------

# DTOs

Criar:

-   PersonaUpsertDto
-   PersonaResponseDto
-   PersonaFilterDto

Regras:

-   Os DTOs devem conter **TODOS os campos da tabela**
-   Não remover nenhuma coluna
-   `digital_hstore01` deve ser tratado como `Dictionary<string,string>`

------------------------------------------------------------------------

# Campos obrigatórios

Somente os campos `NOT NULL` devem ser obrigatórios:

    cpf
    sexo_enum
    etnia_enum
    nacionalidade_enum
    nacionalidade_pais
    def_fisica
    def_visual
    def_auditiva
    def_mental
    def_intelectual
    def_reabilitado
    def_cota
    digital_ok

Todos os outros campos são opcionais.

------------------------------------------------------------------------

# Rotas da API

## POST /personas

Recebe **uma lista de personas**.

Exemplo:

``` json
[
  {
    "cpf": "12345678901",
    "nome": "Joao",
    "sexo_enum": 1,
    "etnia_enum": 1
  },
  {
    "cpf": "22222222222",
    "nome": "Maria",
    "sexo_enum": 2,
    "etnia_enum": 1
  }
]
```

### Regra

Para cada persona:

1.  verificar se o CPF já existe
2.  se não existir → INSERT
3.  se existir → UPDATE

Não existe rota separada de update.

Resposta:

``` json
{
  "inserted": 10,
  "updated": 3,
  "errors": []
}
```

------------------------------------------------------------------------

## GET /personas/{cpf}

Retorna todos os dados da persona.

------------------------------------------------------------------------

## GET /personas

Lista todas as personas.

Regras:

-   **Sem paginação**
-   Permite apenas filtros

### Filtros

    cpf
    nome
    municipio_id
    uf
    fkrecadastramento
    recad_status

Exemplos:

    /personas?cpf=12345678901
    /personas?uf=MA

------------------------------------------------------------------------

## GET /personas/export

Exporta todos os registros.

Query param obrigatório:

    format=json
    format=csv
    format=xlsx

------------------------------------------------------------------------

# Exportação

## JSON

Retornar array com todos os campos.

------------------------------------------------------------------------

## CSV

Regras:

    encoding: UTF8
    separator: |
    header: true

Exemplo:

    id|cpf|nome|...

------------------------------------------------------------------------

## XLSX

Usar **ClosedXML**.

Planilha:

    personas

Primeira linha deve conter os nomes das colunas.

------------------------------------------------------------------------

# SQL obrigatório

Não usar `SELECT *`.

Sempre listar explicitamente todas as colunas da tabela.

------------------------------------------------------------------------

# Tratamento de hstore

Configurar Npgsql:

    NpgsqlConnection.GlobalTypeMapper.UseHstore();

`digital_hstore01` deve ser mapeado como:

    Dictionary<string,string>

------------------------------------------------------------------------

# Validação de CPF duplicado

A tabela possui trigger que impede duplicidade.

Mesmo assim a API deve:

1.  verificar antes de inserir
2.  tratar exception do banco

Retornar:

    409 Conflict

------------------------------------------------------------------------

# Repository

Criar `PersonaRepository` com:

    UpsertManyAsync(List<PersonaUpsertDto>)
    GetByCpfAsync
    ListAsync
    ExportAsync

------------------------------------------------------------------------

# Arquivos esperados

    Controllers/PersonaController.cs
    Services/PersonaService.cs
    Repositories/PersonaRepository.cs
    Models/Persona.cs
    DTO/PersonaUpsertDto.cs
    DTO/PersonaResponseDto.cs
    DTO/PersonaFilterDto.cs
    Export/CsvExporter.cs
    Export/XlsxExporter.cs
    Database/PostgresConnectionFactory.cs
    Validators/PersonaValidator.cs
    Program.cs
    appsettings.json
    README.md

------------------------------------------------------------------------

# Requisitos finais

A solução deve:

1.  compilar
2.  usar PostgreSQL
3.  usar Dapper
4.  aceitar lista no POST
5.  fazer UPSERT por CPF
6.  retornar todas as colunas
7.  exportar todas as colunas
8.  não omitir campos
9.  usar separador `|` no CSV
10. exigir apenas campos NOT NULL
