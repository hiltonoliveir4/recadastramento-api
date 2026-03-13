using System.Data;
using System.Text;
using System.Text.Json;
using Dapper;
using RecadastramentoApi.Database;
using RecadastramentoApi.DTO;
using RecadastramentoApi.Models;

namespace RecadastramentoApi.Repositories;

public sealed class PersonaRepository(IDbConnectionFactory connectionFactory) : IPersonaRepository
{
    private const string TableName = "recadastra.recad_persona";

    private const string SelectColumns = """
        SELECT
            id,
            fkrecadastramento,
            nome,
            nome_social,
            fonetico,
            cpf,
            pasep,
            data_nascimento,
            data_falecimento,
            mae,
            pai,
            sexo_enum,
            etnia_enum,
            ddd,
            telefone,
            telefone2,
            e_mail,
            nit,
            nis,
            nacionalidade_enum,
            nacionalidade_pais,
            naturalidade_id,
            cod_est_civil,
            cod_instrucao,
            tiposang_enum,
            rg,
            rg_data,
            rg_cod_emissor,
            rg_uf,
            cnes,
            ctps,
            ctps_serie,
            ctps_uf,
            ctps_data,
            data_opcao,
            ctps_digital,
            titulo,
            titulo_data,
            titulo_zona,
            titulo_secao,
            titulo_fkcidade,
            habilitacao,
            habilitacao_categ_enum,
            habilitacao_validade,
            habilitacao_emissao,
            def_fisica,
            def_visual,
            def_auditiva,
            def_mental,
            def_intelectual,
            def_reabilitado,
            def_cota,
            def_observacao,
            tipo_logradouro,
            logradouro,
            numero,
            complemento,
            cep,
            bairro,
            municipio_id,
            ponto_referencia,
            cod_banco,
            agencia,
            conta,
            conta_op,
            conta_tipo,
            sync_bimestre,
            controle_data_migracao,
            controle_usuario_id,
            controle_data_alteracao,
            controle_verificador,
            controle_cadastrador_id,
            controle_data_cadastro,
            recad_userid,
            recad_data,
            recad_versao,
            recad_status,
            ordem_fkorgao,
            ordem_numero,
            ordem_emissao,
            ordem_validade,
            ordem_observacao,
            uf,
            digital_ok,
            digital_valor,
            digital_imagem,
            hstore_to_json(digital_hstore01)::text AS digital_hstore01
        FROM recadastra.recad_persona
        """;

    private const string InsertSql = """
        INSERT INTO recadastra.recad_persona
        (
            fkrecadastramento,
            nome,
            nome_social,
            fonetico,
            cpf,
            pasep,
            data_nascimento,
            data_falecimento,
            mae,
            pai,
            sexo_enum,
            etnia_enum,
            ddd,
            telefone,
            telefone2,
            e_mail,
            nit,
            nis,
            nacionalidade_enum,
            nacionalidade_pais,
            naturalidade_id,
            cod_est_civil,
            cod_instrucao,
            tiposang_enum,
            rg,
            rg_data,
            rg_cod_emissor,
            rg_uf,
            cnes,
            ctps,
            ctps_serie,
            ctps_uf,
            ctps_data,
            data_opcao,
            ctps_digital,
            titulo,
            titulo_data,
            titulo_zona,
            titulo_secao,
            titulo_fkcidade,
            habilitacao,
            habilitacao_categ_enum,
            habilitacao_validade,
            habilitacao_emissao,
            def_fisica,
            def_visual,
            def_auditiva,
            def_mental,
            def_intelectual,
            def_reabilitado,
            def_cota,
            def_observacao,
            tipo_logradouro,
            logradouro,
            numero,
            complemento,
            cep,
            bairro,
            municipio_id,
            ponto_referencia,
            cod_banco,
            agencia,
            conta,
            conta_op,
            conta_tipo,
            sync_bimestre,
            controle_data_migracao,
            controle_usuario_id,
            controle_data_alteracao,
            controle_verificador,
            controle_cadastrador_id,
            controle_data_cadastro,
            recad_userid,
            recad_data,
            recad_versao,
            recad_status,
            ordem_fkorgao,
            ordem_numero,
            ordem_emissao,
            ordem_validade,
            ordem_observacao,
            uf,
            digital_ok,
            digital_valor,
            digital_imagem,
            digital_hstore01
        )
        VALUES
        (
            @FkRecadastramento,
            @Nome,
            @NomeSocial,
            @Fonetico,
            @Cpf,
            @Pasep,
            @DataNascimento,
            @DataFalecimento,
            @Mae,
            @Pai,
            @SexoEnum,
            @EtniaEnum,
            @Ddd,
            @Telefone,
            @Telefone2,
            @EMail,
            @Nit,
            @Nis,
            @NacionalidadeEnum,
            @NacionalidadePais,
            @NaturalidadeId,
            @CodEstCivil,
            @CodInstrucao,
            @TiposangEnum,
            @Rg,
            @RgData,
            @RgCodEmissor,
            @RgUf,
            @Cnes,
            @Ctps,
            @CtpsSerie,
            @CtpsUf,
            @CtpsData,
            @DataOpcao,
            @CtpsDigital,
            @Titulo,
            @TituloData,
            @TituloZona,
            @TituloSecao,
            @TituloFkcidade,
            @Habilitacao,
            @HabilitacaoCategEnum,
            @HabilitacaoValidade,
            @HabilitacaoEmissao,
            @DefFisica,
            @DefVisual,
            @DefAuditiva,
            @DefMental,
            @DefIntelectual,
            @DefReabilitado,
            @DefCota,
            @DefObservacao,
            @TipoLogradouro,
            @Logradouro,
            @Numero,
            @Complemento,
            @Cep,
            @Bairro,
            @MunicipioId,
            @PontoReferencia,
            @CodBanco,
            @Agencia,
            @Conta,
            @ContaOp,
            @ContaTipo,
            @SyncBimestre,
            @ControleDataMigracao,
            @ControleUsuarioId,
            @ControleDataAlteracao,
            @ControleVerificador,
            @ControleCadastradorId,
            @ControleDataCadastro,
            @RecadUserid,
            @RecadData,
            @RecadVersao,
            @RecadStatus,
            @OrdemFkorgao,
            @OrdemNumero,
            @OrdemEmissao,
            @OrdemValidade,
            @OrdemObservacao,
            @Uf,
            @DigitalOk,
            @DigitalValor,
            @DigitalImagem,
            CASE
                WHEN @DigitalHstore01Json IS NULL THEN NULL
                ELSE (
                    SELECT hstore(array_agg(key), array_agg(value))
                    FROM json_each_text(@DigitalHstore01Json::json)
                )
            END
        );
        """;

    private const string UpdateByCpfSql = """
        UPDATE recadastra.recad_persona SET
            fkrecadastramento = @FkRecadastramento,
            nome = @Nome,
            nome_social = @NomeSocial,
            fonetico = @Fonetico,
            pasep = @Pasep,
            data_nascimento = @DataNascimento,
            data_falecimento = @DataFalecimento,
            mae = @Mae,
            pai = @Pai,
            sexo_enum = @SexoEnum,
            etnia_enum = @EtniaEnum,
            ddd = @Ddd,
            telefone = @Telefone,
            telefone2 = @Telefone2,
            e_mail = @EMail,
            nit = @Nit,
            nis = @Nis,
            nacionalidade_enum = @NacionalidadeEnum,
            nacionalidade_pais = @NacionalidadePais,
            naturalidade_id = @NaturalidadeId,
            cod_est_civil = @CodEstCivil,
            cod_instrucao = @CodInstrucao,
            tiposang_enum = @TiposangEnum,
            rg = @Rg,
            rg_data = @RgData,
            rg_cod_emissor = @RgCodEmissor,
            rg_uf = @RgUf,
            cnes = @Cnes,
            ctps = @Ctps,
            ctps_serie = @CtpsSerie,
            ctps_uf = @CtpsUf,
            ctps_data = @CtpsData,
            data_opcao = @DataOpcao,
            ctps_digital = @CtpsDigital,
            titulo = @Titulo,
            titulo_data = @TituloData,
            titulo_zona = @TituloZona,
            titulo_secao = @TituloSecao,
            titulo_fkcidade = @TituloFkcidade,
            habilitacao = @Habilitacao,
            habilitacao_categ_enum = @HabilitacaoCategEnum,
            habilitacao_validade = @HabilitacaoValidade,
            habilitacao_emissao = @HabilitacaoEmissao,
            def_fisica = @DefFisica,
            def_visual = @DefVisual,
            def_auditiva = @DefAuditiva,
            def_mental = @DefMental,
            def_intelectual = @DefIntelectual,
            def_reabilitado = @DefReabilitado,
            def_cota = @DefCota,
            def_observacao = @DefObservacao,
            tipo_logradouro = @TipoLogradouro,
            logradouro = @Logradouro,
            numero = @Numero,
            complemento = @Complemento,
            cep = @Cep,
            bairro = @Bairro,
            municipio_id = @MunicipioId,
            ponto_referencia = @PontoReferencia,
            cod_banco = @CodBanco,
            agencia = @Agencia,
            conta = @Conta,
            conta_op = @ContaOp,
            conta_tipo = @ContaTipo,
            sync_bimestre = @SyncBimestre,
            controle_data_migracao = @ControleDataMigracao,
            controle_usuario_id = @ControleUsuarioId,
            controle_data_alteracao = @ControleDataAlteracao,
            controle_verificador = @ControleVerificador,
            controle_cadastrador_id = @ControleCadastradorId,
            controle_data_cadastro = @ControleDataCadastro,
            recad_userid = @RecadUserid,
            recad_data = @RecadData,
            recad_versao = @RecadVersao,
            recad_status = @RecadStatus,
            ordem_fkorgao = @OrdemFkorgao,
            ordem_numero = @OrdemNumero,
            ordem_emissao = @OrdemEmissao,
            ordem_validade = @OrdemValidade,
            ordem_observacao = @OrdemObservacao,
            uf = @Uf,
            digital_ok = @DigitalOk,
            digital_valor = @DigitalValor,
            digital_imagem = @DigitalImagem,
            digital_hstore01 = CASE
                WHEN @DigitalHstore01Json IS NULL THEN NULL
                ELSE (
                    SELECT hstore(array_agg(key), array_agg(value))
                    FROM json_each_text(@DigitalHstore01Json::json)
                )
            END
        WHERE cpf = @Cpf;
        """;

    private const string SelectPersonaIdByCpfSql = """
        SELECT id
        FROM recadastra.recad_persona
        WHERE cpf = @Cpf
        LIMIT 1;
        """;

    private const string SelectPersonaIdByIdSql = """
        SELECT id
        FROM recadastra.recad_persona
        WHERE id = @Id
        LIMIT 1;
        """;

    private const string SelectDependenteByLinkSql = """
        SELECT id
        FROM recadastra.recad_dependente
        WHERE fkresponsavel = @FkResponsavel
          AND fkdependente = @FkDependente
        LIMIT 1;
        """;

    private const string InsertDependenteSql = """
        INSERT INTO recadastra.recad_dependente
        (
            fkresponsavel,
            fkdependente,
            fkparentesco,
            fkespecial,
            considerar_irrf,
            cadastro_fkusuario,
            cadastro_data,
            encerramento_fkusuario,
            encerramento_motivo,
            observacao,
            excluirregistro
        )
        VALUES
        (
            @FkResponsavel,
            @FkDependente,
            @FkParentesco,
            COALESCE(@FkEspecial, 0),
            COALESCE(@ConsiderarIrrf, 1),
            @CadastroFkusuario,
            COALESCE(@CadastroData, NOW()),
            @EncerramentoFkusuario,
            @EncerramentoMotivo,
            COALESCE(@Observacao, ''),
            COALESCE(@ExcluirRegistro, false)
        );
        """;

    private const string UpdateDependenteSql = """
        UPDATE recadastra.recad_dependente
        SET
            fkparentesco = @FkParentesco,
            fkespecial = COALESCE(@FkEspecial, 0),
            considerar_irrf = COALESCE(@ConsiderarIrrf, 1),
            cadastro_fkusuario = @CadastroFkusuario,
            cadastro_data = COALESCE(@CadastroData, cadastro_data),
            encerramento_fkusuario = @EncerramentoFkusuario,
            encerramento_motivo = @EncerramentoMotivo,
            observacao = COALESCE(@Observacao, ''),
            excluirregistro = COALESCE(@ExcluirRegistro, false)
        WHERE id = @Id;
        """;

    private const string SelectConjugeByLinkSql = """
        SELECT id
        FROM recadastra.recad_conjuge
        WHERE fkpersona = @FkPersona
          AND fkconjuge = @FkConjuge
        LIMIT 1;
        """;

    private const string SelectAnexoByLinkSql = """
        SELECT id
        FROM recadastra.recad_anexo
        WHERE fkpersona = @FkPersona
          AND fkanexotipo = @FkAnexoTipo
        ORDER BY id DESC
        LIMIT 1;
        """;

    private const string InsertAnexoSql = """
        INSERT INTO recadastra.recad_anexo
        (
            fkpersona,
            fkanexotipo,
            obrigatorio,
            emissao_data,
            validade_data,
            cadastro_fkusuario,
            cadastro_data,
            url,
            arquivo,
            observacao
        )
        VALUES
        (
            @FkPersona,
            @FkAnexoTipo,
            COALESCE(@Obrigatorio, 0),
            @EmissaoData,
            @ValidadeData,
            @CadastroFkusuario,
            COALESCE(@CadastroData, NOW()),
            @Url,
            @Arquivo,
            COALESCE(@Observacao, '')
        );
        """;

    private const string UpdateAnexoSql = """
        UPDATE recadastra.recad_anexo
        SET
            obrigatorio = COALESCE(@Obrigatorio, obrigatorio),
            emissao_data = COALESCE(@EmissaoData, emissao_data),
            validade_data = COALESCE(@ValidadeData, validade_data),
            cadastro_fkusuario = COALESCE(@CadastroFkusuario, cadastro_fkusuario),
            cadastro_data = COALESCE(@CadastroData, cadastro_data),
            url = COALESCE(@Url, url),
            arquivo = COALESCE(@Arquivo, arquivo),
            observacao = COALESCE(@Observacao, observacao)
        WHERE id = @Id;
        """;

    private const string InsertConjugeSql = """
        INSERT INTO recadastra.recad_conjuge
        (
            fkpersona,
            fkconjuge,
            fkregimecasamento,
            datacasamento,
            cadastro_fkusuario,
            cadastro_data,
            encerramento_fkusuario,
            encerramento_data,
            encerramento_motivo,
            observacao,
            excluirregistro
        )
        VALUES
        (
            @FkPersona,
            @FkConjuge,
            @FkRegimeCasamento,
            @DataCasamento,
            @CadastroFkusuario,
            COALESCE(@CadastroData, NOW()),
            @EncerramentoFkusuario,
            @EncerramentoData,
            @EncerramentoMotivo,
            COALESCE(@Observacao, ''),
            COALESCE(@ExcluirRegistro, false)
        );
        """;

    private const string UpdateConjugeSql = """
        UPDATE recadastra.recad_conjuge
        SET
            fkregimecasamento = @FkRegimeCasamento,
            datacasamento = @DataCasamento,
            cadastro_fkusuario = @CadastroFkusuario,
            cadastro_data = COALESCE(@CadastroData, cadastro_data),
            encerramento_fkusuario = @EncerramentoFkusuario,
            encerramento_data = @EncerramentoData,
            encerramento_motivo = @EncerramentoMotivo,
            observacao = COALESCE(@Observacao, ''),
            excluirregistro = COALESCE(@ExcluirRegistro, false)
        WHERE id = @Id;
        """;

    public async Task<Persona?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var sql = $"{SelectColumns} WHERE cpf = @Cpf";
        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { Cpf = cpf }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Persona>(command);
    }

    public async Task<IReadOnlyList<Persona>> GetAllAsync(PersonaFilterDto filter, CancellationToken cancellationToken = default)
    {
        var sqlBuilder = new StringBuilder(SelectColumns);
        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(filter.Cpf))
        {
            whereClauses.Add("cpf = @Cpf");
            parameters.Add("Cpf", filter.Cpf);
        }

        if (!string.IsNullOrWhiteSpace(filter.Nome))
        {
            whereClauses.Add("nome ILIKE @Nome");
            parameters.Add("Nome", $"%{filter.Nome}%");
        }

        if (filter.MunicipioId.HasValue)
        {
            whereClauses.Add("municipio_id = @MunicipioId");
            parameters.Add("MunicipioId", filter.MunicipioId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Uf))
        {
            whereClauses.Add("uf = @Uf");
            parameters.Add("Uf", filter.Uf);
        }

        if (filter.FkRecadastramento.HasValue)
        {
            whereClauses.Add("fkrecadastramento = @FkRecadastramento");
            parameters.Add("FkRecadastramento", filter.FkRecadastramento.Value);
        }

        if (filter.RecadStatus.HasValue)
        {
            whereClauses.Add("recad_status = @RecadStatus");
            parameters.Add("RecadStatus", filter.RecadStatus.Value);
        }

        if (whereClauses.Count > 0)
        {
            sqlBuilder.Append(" WHERE ");
            sqlBuilder.Append(string.Join(" AND ", whereClauses));
        }

        sqlBuilder.Append(" ORDER BY id");

        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
        var personas = await connection.QueryAsync<Persona>(command);
        return personas.ToList();
    }

    public async Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT EXISTS (SELECT 1 FROM {TableName} WHERE cpf = @Cpf)";

        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Cpf = cpf }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<long?> GetIdByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(SelectPersonaIdByCpfSql, new { Cpf = cpf }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<long?>(command);
    }

    public async Task InsertAsync(PersonaUpsertDto dto, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(InsertSql, BuildParameters(dto), cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }

    public async Task UpdateByCpfAsync(PersonaUpsertDto dto, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(UpdateByCpfSql, BuildParameters(dto), cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }

    public async Task UpsertAnexosAsync(long fkPersona, IReadOnlyList<AnexoUpsertDto> anexos, CancellationToken cancellationToken = default)
    {
        if (anexos.Count == 0)
        {
            return;
        }

        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        foreach (var anexo in anexos)
        {
            var linkIdCommand = new CommandDefinition(
                SelectAnexoByLinkSql,
                new { FkPersona = fkPersona, FkAnexoTipo = anexo.FkAnexoTipo },
                cancellationToken: cancellationToken);

            var existingLinkId = await connection.QuerySingleOrDefaultAsync<long?>(linkIdCommand);

            var parameters = new DynamicParameters();
            parameters.Add("Id", existingLinkId);
            parameters.Add("FkPersona", fkPersona);
            parameters.Add("FkAnexoTipo", anexo.FkAnexoTipo);
            parameters.Add("Obrigatorio", anexo.Obrigatorio);
            parameters.Add("EmissaoData", anexo.EmissaoData);
            parameters.Add("ValidadeData", anexo.ValidadeData);
            parameters.Add("CadastroFkusuario", anexo.CadastroFkusuario);
            parameters.Add("CadastroData", anexo.CadastroData);
            parameters.Add("Url", anexo.Url, DbType.String);
            parameters.Add("Arquivo", anexo.Arquivo, DbType.Binary);
            parameters.Add("Observacao", anexo.Observacao, DbType.String);

            var sql = existingLinkId.HasValue ? UpdateAnexoSql : InsertAnexoSql;
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task UpsertDependentesAsync(long fkResponsavel, IReadOnlyList<DependenteUpsertDto> dependentes, CancellationToken cancellationToken = default)
    {
        if (dependentes.Count == 0)
        {
            return;
        }

        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        foreach (var dependente in dependentes)
        {
            var fkDependente = await ResolveLinkedPersonaIdAsync(
                connection,
                dependente.FkDependente,
                dependente.CpfDependente,
                "dependente",
                cancellationToken);

            var linkIdCommand = new CommandDefinition(
                SelectDependenteByLinkSql,
                new { FkResponsavel = fkResponsavel, FkDependente = fkDependente },
                cancellationToken: cancellationToken);

            var existingLinkId = await connection.QuerySingleOrDefaultAsync<long?>(linkIdCommand);

            var parameters = new
            {
                Id = existingLinkId,
                FkResponsavel = fkResponsavel,
                FkDependente = fkDependente,
                FkParentesco = dependente.FkParentesco,
                FkEspecial = dependente.FkEspecial,
                ConsiderarIrrf = dependente.ConsiderarIrrf,
                CadastroFkusuario = dependente.CadastroFkusuario,
                CadastroData = dependente.CadastroData,
                EncerramentoFkusuario = dependente.EncerramentoFkusuario,
                EncerramentoMotivo = dependente.EncerramentoMotivo,
                Observacao = dependente.Observacao,
                ExcluirRegistro = dependente.ExcluirRegistro
            };

            var sql = existingLinkId.HasValue ? UpdateDependenteSql : InsertDependenteSql;
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task UpsertConjugesAsync(long fkPersona, IReadOnlyList<ConjugeUpsertDto> conjuges, CancellationToken cancellationToken = default)
    {
        if (conjuges.Count == 0)
        {
            return;
        }

        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        foreach (var conjuge in conjuges)
        {
            var fkConjuge = await ResolveLinkedPersonaIdAsync(
                connection,
                conjuge.FkConjuge,
                conjuge.CpfConjuge,
                "conjuge",
                cancellationToken);

            var linkIdCommand = new CommandDefinition(
                SelectConjugeByLinkSql,
                new { FkPersona = fkPersona, FkConjuge = fkConjuge },
                cancellationToken: cancellationToken);

            var existingLinkId = await connection.QuerySingleOrDefaultAsync<long?>(linkIdCommand);

            var parameters = new
            {
                Id = existingLinkId,
                FkPersona = fkPersona,
                FkConjuge = fkConjuge,
                FkRegimeCasamento = conjuge.FkRegimeCasamento,
                DataCasamento = conjuge.DataCasamento,
                CadastroFkusuario = conjuge.CadastroFkusuario,
                CadastroData = conjuge.CadastroData,
                EncerramentoFkusuario = conjuge.EncerramentoFkusuario,
                EncerramentoData = conjuge.EncerramentoData,
                EncerramentoMotivo = conjuge.EncerramentoMotivo,
                Observacao = conjuge.Observacao,
                ExcluirRegistro = conjuge.ExcluirRegistro
            };

            var sql = existingLinkId.HasValue ? UpdateConjugeSql : InsertConjugeSql;
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    private static DynamicParameters BuildParameters(PersonaUpsertDto dto)
    {
        var parameters = new DynamicParameters(dto);
        parameters.Add(
            "DigitalHstore01Json",
            dto.DigitalHstore01 is null ? null : JsonSerializer.Serialize(dto.DigitalHstore01),
            DbType.String);
        return parameters;
    }

    private static async Task<long> ResolveLinkedPersonaIdAsync(
        IDbConnection connection,
        long? fkPersona,
        string? cpfPersona,
        string relationName,
        CancellationToken cancellationToken)
    {
        if (fkPersona.HasValue && fkPersona.Value > 0)
        {
            var idCommand = new CommandDefinition(
                SelectPersonaIdByIdSql,
                new { Id = fkPersona.Value },
                cancellationToken: cancellationToken);

            var existingPersonaId = await connection.QuerySingleOrDefaultAsync<long?>(idCommand);
            if (!existingPersonaId.HasValue)
            {
                throw new InvalidOperationException($"Linked {relationName} persona not found for id {fkPersona.Value}.");
            }

            return existingPersonaId.Value;
        }

        if (string.IsNullOrWhiteSpace(cpfPersona))
        {
            throw new InvalidOperationException($"Either fk_{relationName} or cpf_{relationName} must be provided.");
        }

        var command = new CommandDefinition(
            SelectPersonaIdByCpfSql,
            new { Cpf = cpfPersona.Trim() },
            cancellationToken: cancellationToken);

        var linkedPersonaId = await connection.QuerySingleOrDefaultAsync<long?>(command);
        if (!linkedPersonaId.HasValue)
        {
            throw new InvalidOperationException($"Linked {relationName} persona not found for cpf {cpfPersona.Trim()}.");
        }

        return linkedPersonaId.Value;
    }
}
