using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace WindowsFormsApp2.interfaces
{
    public interface IPersistencia
    {
        void InserirGenerico(string tabela, Dictionary<string, object> dados);
        MySqlConnection ConnectTo();
    }
}
