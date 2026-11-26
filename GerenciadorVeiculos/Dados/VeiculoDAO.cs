using MySql.Data.MySqlClient;
using GerenciadorVeiculos.Modelos;
using System;
using System.Data;

namespace GerenciadorVeiculos.Dados
{
    public class VeiculoDAO
    {
        // CREATE: insere um novo veículo no banco de dados
        public void Cadastrar(Veiculo veiculo)
        {
            string tipo = "";
            int dadoEspecifico = 0;

            if (veiculo is Carro carro)
            {
                tipo = "Carro";
                dadoEspecifico = carro.Portas;
            }
            else if (veiculo is Moto moto)
            {
                tipo = "Moto";
                dadoEspecifico = moto.Cilindradas;
            }

            string sql = "INSERT INTO veiculos (modelo, tipo, valor_diaria, valor_seguro, dado_especifico) " +
                         "VALUES (@modelo, @tipo, @valor_diaria, @valor_seguro, @dado_especifico);";

            using (MySqlConnection conexao = ConexaoBD.ObterConexao())
            {
                using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                {
                    comando.Parameters.AddWithValue("@modelo", veiculo.Modelo);
                    comando.Parameters.AddWithValue("@tipo", tipo);
                    comando.Parameters.AddWithValue("@valor_diaria", veiculo.ValorDiaria);
                    comando.Parameters.AddWithValue("@valor_seguro", veiculo.CalcularSeguro());
                    comando.Parameters.AddWithValue("@dado_especifico", dadoEspecifico);

                    comando.ExecuteNonQuery();
                }
            }
        }

        // READ: busca todos os veículos cadastrados e devolve uma tabela (DataTable)
        public DataTable ListarTodos()
        {
            string sql = "SELECT id AS 'ID', modelo AS 'Modelo', tipo AS 'Tipo', " +
                         "valor_diaria AS 'Diária (R$)', valor_seguro AS 'Seguro (R$)', " +
                         "dado_especifico AS 'Portas / Cilindradas' " +
                         "FROM veiculos ORDER BY id DESC;";

            using (MySqlConnection conexao = ConexaoBD.ObterConexao())
            {
                using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                {
                    using (MySqlDataAdapter adaptador = new MySqlDataAdapter(comando))
                    {
                        DataTable tabela = new DataTable();
                        adaptador.Fill(tabela);
                        return tabela;
                    }
                }
            }
        }

        // UPDATE: atualiza os dados de um veículo existente com base no ID
        public void Atualizar(Veiculo veiculo)
        {
            string tipo = "";
            int dadoEspecifico = 0;

            if (veiculo is Carro carro)
            {
                tipo = "Carro";
                dadoEspecifico = carro.Portas;
            }
            else if (veiculo is Moto moto)
            {
                tipo = "Moto";
                dadoEspecifico = moto.Cilindradas;
            }

            string sql = "UPDATE veiculos SET modelo = @modelo, tipo = @tipo, " +
                         "valor_diaria = @valor_diaria, valor_seguro = @valor_seguro, " +
                         "dado_especifico = @dado_especifico " +
                         "WHERE id = @id;";

            using (MySqlConnection conexao = ConexaoBD.ObterConexao())
            {
                using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                {
                    comando.Parameters.AddWithValue("@modelo", veiculo.Modelo);
                    comando.Parameters.AddWithValue("@tipo", tipo);
                    comando.Parameters.AddWithValue("@valor_diaria", veiculo.ValorDiaria);
                    comando.Parameters.AddWithValue("@valor_seguro", veiculo.CalcularSeguro());
                    comando.Parameters.AddWithValue("@dado_especifico", dadoEspecifico);
                    comando.Parameters.AddWithValue("@id", veiculo.Id);

                    comando.ExecuteNonQuery();
                }
            }
        }

        // DELETE: remove um veículo do banco de dados com base no ID
        public void Excluir(int id)
        {
            string sql = "DELETE FROM veiculos WHERE id = @id;";

            using (MySqlConnection conexao = ConexaoBD.ObterConexao())
            {
                using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    comando.ExecuteNonQuery();
                }
            }
        }
    }
}