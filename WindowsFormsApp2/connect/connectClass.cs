using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using WindowsFormsApp2.interfaces;

public class ConnectClass : IPersistencia
{
    private const string connectStr = "Server=localhost;Database=controle_db;Uid=root;Pwd=;";

    // Método para obter conexão aberta
    public MySqlConnection ConnectTo()
    {
        var connection = new MySqlConnection(connectStr);
        connection.Open();
        return connection;
    }

    public void InserirGenerico(string tabela, Dictionary<string, object> dados)
    {
        string colunas = string.Join(", ", dados.Keys);
        string parametros = string.Join(", ", dados.Keys.Select(k => "@" + k));
        string sql = $"INSERT INTO {tabela} ({colunas}) VALUES ({parametros})";

        using (var conexao = ConnectTo())
        using (var comando = new MySqlCommand(sql, conexao))
        {
            foreach (var item in dados)
            {
                comando.Parameters.AddWithValue("@" + item.Key, item.Value ?? DBNull.Value);
            }

            try
            {
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir: {ex.Message}");
            }
        }
    }
}
