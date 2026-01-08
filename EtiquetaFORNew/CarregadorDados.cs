using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace EtiquetaFORNew.Data
{
    /// <summary>
    /// Gerenciador de carregamento de produtos por diferentes tipos
    /// Equivalente às queries do SoftShop: GeradordeEtiquetas_Carregar*
    /// </summary>
    public static class CarregadorDados
    {
        // ⭐ ConnectionString local (mesmo do LocalDatabaseManager)
        private static readonly string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "LocalData.db");
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";
        // ========================================
        // 🔹 CARREGAMENTO POR TIPO
        // ========================================

        /// <summary>
        /// Carrega produtos baseado no tipo e filtros fornecidos
        /// </summary>
        public static DataTable CarregarProdutosPorTipo(
            string tipo,
            string documento = null,
            DateTime? dataInicial = null,
            DateTime? dataFinal = null,
            string grupo = null,
            string subGrupo = null,
            string fabricante = null,
            string fornecedor = null,
            string produto = null,
            bool isConfeccao = false,
            int? idPromocao = null) // ⭐ NOVO parâmetro
        {
            switch (tipo.ToUpper())
            {
                case "AJUSTES":
                    return CarregarAjustes(documento, dataInicial, dataFinal);

                case "BALANÇOS":
                    return CarregarBalancos(documento, dataInicial, dataFinal);

                case "NOTAS ENTRADA":
                    return CarregarNotasEntrada(documento, dataInicial, dataFinal);

                case "PREÇOS ALTERADOS":
                    return CarregarPrecosAlterados(dataInicial.Value, dataFinal.Value);

                case "PROMOÇÕES":
                    // ⭐ Usa o método do PromocoesManager com ID da promoção
                    if (idPromocao.HasValue)
                    {
                        return PromocoesManager.BuscarProdutosDaPromocao(
                            idPromocao.Value,
                            null, // loja (usa padrão)
                            produto,
                            grupo,
                            subGrupo,
                            fabricante,
                            fornecedor);
                    }
                    else
                    {
                        throw new Exception("ID da promoção não foi informado!");
                    }

                case "FILTROS MANUAIS":
                default:
                    // Para filtros manuais, usa o método existente do LocalDatabaseManager
                    // que aceita: grupo, fabricante, fornecedor, isConfeccao
                    return LocalDatabaseManager.BuscarMercadoriasPorFiltros(
                        grupo,
                        fabricante,
                        fornecedor,
                        isConfeccao);
            }
        }

        // ========================================
        // 🔹 AJUSTES DE ESTOQUE
        // ========================================
        /// <summary>
        /// Carrega produtos de ajustes de estoque
        /// Equivalente: GeradordeEtiquetas_CarregarAjustes
        /// </summary>
        private static DataTable CarregarAjustes(string numeroAjuste, DateTime? dataInicial, DateTime? dataFinal)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT
                            m.CodigoMercadoria,
                            m.Mercadoria,
                            m.PrecoVenda,
                            m.Grupo,
                            m.SubGrupo,
                            m.Fabricante,
                            m.Fornecedor,
                            m.CodBarras,
                            m.CodFabricante,
                            m.Tam,
                            m.Cores,
                            m.CodBarras_Grade,
                            m.Registro,
                            1 as Quantidade
                        FROM Mercadorias m
                        WHERE 1=1
                    ";

                    List<string> condicoes = new List<string>();
                    var parametros = new List<SQLiteParameter>();

                    // Filtro por número do ajuste (se implementado em campo específico)
                    if (!string.IsNullOrEmpty(numeroAjuste))
                    {
                        // TODO: Implementar quando houver campo de controle de ajustes
                        // condicoes.Add("m.NumeroAjuste = @numeroAjuste");
                        // parametros.Add(new SQLiteParameter("@numeroAjuste", numeroAjuste));
                    }

                    // Filtro por data
                    if (dataInicial.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de ajuste
                        // condicoes.Add("DATE(m.DataAjuste) >= DATE(@dataInicial)");
                        // parametros.Add(new SQLiteParameter("@dataInicial", dataInicial.Value.ToString("yyyy-MM-dd")));
                    }

                    if (dataFinal.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de ajuste
                        // condicoes.Add("DATE(m.DataAjuste) <= DATE(@dataFinal)");
                        // parametros.Add(new SQLiteParameter("@dataFinal", dataFinal.Value.ToString("yyyy-MM-dd")));
                    }

                    if (condicoes.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", condicoes);
                    }

                    query += " ORDER BY m.Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        foreach (var param in parametros)
                        {
                            cmd.Parameters.Add(param);
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar ajustes: {ex.Message}", ex);
            }
        }

        // ========================================
        // 🔹 BALANÇOS
        // ========================================
        /// <summary>
        /// Carrega produtos de balanços de estoque
        /// Equivalente: GeradordeEtiquetas_CarregarBalancos
        /// </summary>
        private static DataTable CarregarBalancos(string numeroBalanco, DateTime? dataInicial, DateTime? dataFinal)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT
                            m.CodigoMercadoria,
                            m.Mercadoria,
                            m.PrecoVenda,
                            m.Grupo,
                            m.SubGrupo,
                            m.Fabricante,
                            m.Fornecedor,
                            m.CodBarras,
                            m.CodFabricante,
                            m.Tam,
                            m.Cores,
                            m.CodBarras_Grade,
                            m.Registro,
                            1 as Quantidade
                        FROM Mercadorias m
                        WHERE 1=1
                    ";

                    List<string> condicoes = new List<string>();
                    var parametros = new List<SQLiteParameter>();

                    // Filtro por número do balanço
                    if (!string.IsNullOrEmpty(numeroBalanco))
                    {
                        // TODO: Implementar quando houver campo de controle de balanços
                        // condicoes.Add("m.NumeroBalanco = @numeroBalanco");
                        // parametros.Add(new SQLiteParameter("@numeroBalanco", numeroBalanco));
                    }

                    // Filtro por data
                    if (dataInicial.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de balanço
                    }

                    if (dataFinal.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de balanço
                    }

                    if (condicoes.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", condicoes);
                    }

                    query += " ORDER BY m.Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        foreach (var param in parametros)
                        {
                            cmd.Parameters.Add(param);
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar balanços: {ex.Message}", ex);
            }
        }

        // ========================================
        // 🔹 NOTAS DE ENTRADA
        // ========================================
        /// <summary>
        /// Carrega produtos de notas fiscais de entrada
        /// Equivalente: GeradordeEtiquetas_CarregarCompras
        /// </summary>
        private static DataTable CarregarNotasEntrada(string numeroNF, DateTime? dataInicial, DateTime? dataFinal)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    // Query busca produtos com base em campos que indicam entrada
                    // Adapte conforme estrutura real do banco
                    string query = @"
                        SELECT DISTINCT
                            m.CodigoMercadoria,
                            m.Mercadoria,
                            m.PrecoVenda,
                            m.Grupo,
                            m.SubGrupo,
                            m.Fabricante,
                            m.Fornecedor,
                            m.CodBarras,
                            m.CodFabricante,
                            m.Tam,
                            m.Cores,
                            m.CodBarras_Grade,
                            m.Registro,
                            1 as Quantidade
                        FROM Mercadorias m
                        WHERE 1=1
                    ";

                    List<string> condicoes = new List<string>();
                    var parametros = new List<SQLiteParameter>();

                    // Filtro por NF (quando houver campo específico)
                    if (!string.IsNullOrEmpty(numeroNF))
                    {
                        // TODO: Implementar quando houver tabela de NFs ou campo específico
                        // condicoes.Add("m.NumeroNF = @numeroNF");
                        // parametros.Add(new SQLiteParameter("@numeroNF", numeroNF));
                    }

                    // Filtro por data de entrada
                    if (dataInicial.HasValue)
                    {
                        // TODO: Implementar quando houver campo DataEntrada ou similar
                        // condicoes.Add("DATE(m.DataEntrada) >= DATE(@dataInicial)");
                        // parametros.Add(new SQLiteParameter("@dataInicial", dataInicial.Value.ToString("yyyy-MM-dd")));
                    }

                    if (dataFinal.HasValue)
                    {
                        // TODO: Implementar quando houver campo DataEntrada ou similar
                        // condicoes.Add("DATE(m.DataEntrada) <= DATE(@dataFinal)");
                        // parametros.Add(new SQLiteParameter("@dataFinal", dataFinal.Value.ToString("yyyy-MM-dd")));
                    }

                    if (condicoes.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", condicoes);
                    }

                    query += " ORDER BY m.Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        foreach (var param in parametros)
                        {
                            cmd.Parameters.Add(param);
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar notas de entrada: {ex.Message}", ex);
            }
        }

        // ========================================
        // 🔹 PREÇOS ALTERADOS
        // ========================================
        /// <summary>
        /// Carrega produtos com preços alterados no período
        /// Equivalente: GeradordeEtiquetas_CarregarAlteracaoPrecos
        /// </summary>
        private static DataTable CarregarPrecosAlterados(DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT
                            m.CodigoMercadoria,
                            m.Mercadoria,
                            m.PrecoVenda,
                            m.Grupo,
                            m.SubGrupo,
                            m.Fabricante,
                            m.Fornecedor,
                            m.CodBarras,
                            m.CodFabricante,
                            m.Tam,
                            m.Cores,
                            m.CodBarras_Grade,
                            m.Registro,
                            1 as Quantidade
                        FROM Mercadorias m
                        WHERE 1=1
                    ";

                    // TODO: Implementar quando houver campo de data de alteração de preço
                    // query += @"
                    //     AND DATE(m.DataAlteracaoPreco) >= DATE(@dataInicial)
                    //     AND DATE(m.DataAlteracaoPreco) <= DATE(@dataFinal)
                    // ";

                    query += " ORDER BY m.Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dataInicial", dataInicial.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@dataFinal", dataFinal.ToString("yyyy-MM-dd"));

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar preços alterados: {ex.Message}", ex);
            }
        }

        // ========================================
        // 🔹 PROMOÇÕES
        // ========================================
        /// <summary>
        /// Carrega produtos em promoção com filtros específicos
        /// Equivalente: Promocoes_GeradorEtiquetasAnexar
        /// </summary>
        private static DataTable CarregarPromocoes(
            string grupo = null,
            string subGrupo = null,
            string fabricante = null,
            string fornecedor = null,
            string produto = null)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT
                            m.CodigoMercadoria,
                            m.Mercadoria,
                            m.PrecoVenda,
                            m.Grupo,
                            m.SubGrupo,
                            m.Fabricante,
                            m.Fornecedor,
                            m.CodBarras,
                            m.CodFabricante,
                            m.Tam,
                            m.Cores,
                            m.CodBarras_Grade,
                            m.Registro,
                            1 as Quantidade
                        FROM Mercadorias m
                        WHERE 1=1
                    ";

                    // TODO: Quando houver tabela de promoções:
                    // query += " INNER JOIN Promocoes p ON m.CodigoMercadoria = p.CodigoMercadoria";
                    // query += " WHERE p.Ativa = 1";

                    List<string> condicoes = new List<string>();
                    var parametros = new List<SQLiteParameter>();

                    if (!string.IsNullOrEmpty(grupo))
                    {
                        condicoes.Add("m.Grupo = @grupo");
                        parametros.Add(new SQLiteParameter("@grupo", grupo));
                    }

                    if (!string.IsNullOrEmpty(subGrupo))
                    {
                        condicoes.Add("m.SubGrupo = @subGrupo");
                        parametros.Add(new SQLiteParameter("@subGrupo", subGrupo));
                    }

                    if (!string.IsNullOrEmpty(fabricante))
                    {
                        condicoes.Add("m.Fabricante = @fabricante");
                        parametros.Add(new SQLiteParameter("@fabricante", fabricante));
                    }

                    if (!string.IsNullOrEmpty(fornecedor))
                    {
                        condicoes.Add("m.Fornecedor = @fornecedor");
                        parametros.Add(new SQLiteParameter("@fornecedor", fornecedor));
                    }

                    if (!string.IsNullOrEmpty(produto))
                    {
                        condicoes.Add("m.Mercadoria LIKE @produto");
                        parametros.Add(new SQLiteParameter("@produto", $"%{produto}%"));
                    }

                    if (condicoes.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", condicoes);
                    }

                    query += " ORDER BY m.Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        foreach (var param in parametros)
                        {
                            cmd.Parameters.Add(param);
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar promoções: {ex.Message}", ex);
            }
        }

        // ========================================
        // 🔹 LIMPAR ETIQUETAS EXISTENTES
        // ========================================
        /// <summary>
        /// Limpa produtos já carregados (equivalente ao DELETE no SoftShop)
        /// </summary>
        public static bool LimparEtiquetasCarregadas()
        {
            // Esta funcionalidade pode ser implementada se houver
            // uma "área de staging" para produtos carregados
            // Por enquanto, apenas retorna true
            return true;
        }
    }
}