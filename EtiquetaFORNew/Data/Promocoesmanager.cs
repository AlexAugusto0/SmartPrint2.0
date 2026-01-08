using System;
using System.Data;
using System.Data.SqlClient;

namespace EtiquetaFORNew.Data
{
    /// <summary>
    /// Gerenciador de Promoções - VERSÃO FINAL
    /// Relacionamento: Promocoes ← INNER JOIN → Promocoes_Ativas
    /// Estrutura de retorno compatível com LocalDatabaseManager
    /// </summary>
    public static class PromocoesManager
    {
        /// <summary>
        /// Busca promoções ativas no SQL Server
        /// Da tabela Promocoes (que tem Descricao)
        /// </summary>
        public static DataTable BuscarPromocoesAtivas()
        {
            try
            {
                string connectionString = DatabaseConfig.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Configuração de conexão não encontrada!");
                }

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query: Busca na tabela Promocoes
                    // Filtra apenas promoções que têm produtos ativos em Promocoes_Ativas
                    string query = @"
                        SELECT DISTINCT
                            p.ID_Promocao,
                            p.Descricao
                        FROM Promocoes p
                        INNER JOIN Promocoes_Ativas pa 
                            ON p.ID_Promocao = pa.ID_Promocao
                        ORDER BY p.Descricao
                    ";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            System.Diagnostics.Debug.WriteLine($"✅ Promoções encontradas: {dt.Rows.Count}");

                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao buscar promoções: {ex.Message}");

                // Retorna tabela vazia em caso de erro
                DataTable dtEmpty = new DataTable();
                dtEmpty.Columns.Add("ID_Promocao", typeof(int));
                dtEmpty.Columns.Add("Descricao", typeof(string));

                return dtEmpty;
            }
        }

        /// <summary>
        /// Busca produtos de uma promoção específica com filtros
        /// Retorna estrutura compatível com LocalDatabaseManager.BuscarMercadoriasPorFiltros
        /// </summary>
        public static DataTable BuscarProdutosDaPromocao(
            int idPromocao,
            string loja = null,
            string produto = null,
            string grupo = null,
            string subGrupo = null,
            string fabricante = null,
            string fornecedor = null)
        {
            try
            {
                string connectionString = DatabaseConfig.GetConnectionString();
                DatabaseConfig.ConfigData config = DatabaseConfig.LoadConfiguration();

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Configuração de conexão não encontrada!");
                }

                // Se loja não foi informada, usa a loja configurada
                if (string.IsNullOrEmpty(loja))
                {
                    loja = config.Loja;
                }

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query com estrutura COMPATÍVEL com LocalDatabaseManager
                    // Campos na mesma ordem e nomes que o sistema espera
                    string query = @"
                        SELECT DISTINCT
                            pa.[Código da Mercadoria] as CodigoMercadoria,
                            ISNULL(cm.[Cód Fabricante], '') as CodFabricante,
                            ISNULL(cm.[Cód Barra], '') as CodBarras,
                            pa.[Mercadoria],
                            pa.[Preço de Venda] as PrecoVenda,
                            CAST(0 AS DECIMAL(10,2)) as VendaA,
                            CAST(0 AS DECIMAL(10,2)) as VendaB,
                            CAST(0 AS DECIMAL(10,2)) as VendaC,
                            CAST(0 AS DECIMAL(10,2)) as VendaD,
                            CAST(0 AS DECIMAL(10,2)) as VendaE,
                            ISNULL(cm.[Fornecedor], '') as Fornecedor,
                            ISNULL(cm.[Fabricante], '') as Fabricante,
                            ISNULL(cm.[Grupo], '') as Grupo,
                            '' as Prateleira,
                            '' as Garantia,
                            ISNULL(cml.[Tam], '') as Tam,
                            ISNULL(cml.[Cores], '') as Cores,
                            ISNULL(cml.[CodBarras], '') as CodBarras_Grade,
                            CAST(cml.[Código da Mercadoria] AS VARCHAR) + '-' + ISNULL(cml.[Tam], '') + '-' + ISNULL(cml.[Cores], '') as Registro
                        FROM Promocoes_Ativas pa
                        INNER JOIN [Cadastro de Mercadorias] cm 
                            ON pa.[Código da Mercadoria] = cm.[Código da Mercadoria]
                        INNER JOIN [Cadastro de mercadoriasLojas] cml 
                            ON cm.[Código da Mercadoria] = cml.[Código da Mercadoria]
                        WHERE pa.ID_Promocao = @idPromocao
                        AND cml.Loja = @loja
                    ";

                    // Adicionar filtros opcionais
                    if (!string.IsNullOrEmpty(produto))
                    {
                        query += @"
                            AND (
                                CAST(pa.[Código da Mercadoria] AS VARCHAR) LIKE @produto
                                OR cm.[Cód Fabricante] LIKE @produto
                                OR cm.[Cód Barra] LIKE @produto
                                OR pa.[Mercadoria] LIKE @produto
                            )
                        ";
                    }

                    if (!string.IsNullOrEmpty(grupo))
                    {
                        query += " AND ISNULL(cm.[Grupo], 'VAZIO') LIKE @grupo";
                    }

                    if (!string.IsNullOrEmpty(subGrupo))
                    {
                        query += " AND ISNULL(cm.[SubGrupo], 'VAZIO') LIKE @subGrupo";
                    }

                    if (!string.IsNullOrEmpty(fabricante))
                    {
                        query += " AND ISNULL(cm.[Fabricante], 'VAZIO') LIKE @fabricante";
                    }

                    if (!string.IsNullOrEmpty(fornecedor))
                    {
                        query += " AND ISNULL(cm.[Fornecedor], 'VAZIO') LIKE @fornecedor";
                    }

                    query += " ORDER BY pa.[Mercadoria]";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idPromocao", idPromocao);
                        cmd.Parameters.AddWithValue("@loja", loja);

                        if (!string.IsNullOrEmpty(produto))
                            cmd.Parameters.AddWithValue("@produto", $"%{produto}%");

                        if (!string.IsNullOrEmpty(grupo))
                            cmd.Parameters.AddWithValue("@grupo", grupo);

                        if (!string.IsNullOrEmpty(subGrupo))
                            cmd.Parameters.AddWithValue("@subGrupo", subGrupo);

                        if (!string.IsNullOrEmpty(fabricante))
                            cmd.Parameters.AddWithValue("@fabricante", fabricante);

                        if (!string.IsNullOrEmpty(fornecedor))
                            cmd.Parameters.AddWithValue("@fornecedor", fornecedor);

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            System.Diagnostics.Debug.WriteLine($"✅ Produtos da promoção {idPromocao}: {dt.Rows.Count}");

                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar produtos da promoção: {ex.Message}", ex);
            }
        }
    }
}