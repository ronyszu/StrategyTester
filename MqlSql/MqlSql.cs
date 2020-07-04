using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace MqlSql
{
    public static class MqlSql
    {
        private static SqlConnection conn = null;
        private static SqlCommand com = null;
        private static string sMessage = string.Empty;
        public const int iResSuccess = 0;
        public const int iResError = 1;


        [DllExport("CreateConnection", CallingConvention = CallingConvention.StdCall)]
        public static int CreateConnection(
        [MarshalAs(UnmanagedType.LPWStr)] string sConnStr)
        {
            // Limpamos a linha de mensagem:
            sMessage = string.Empty;
            // Se já houver conexão, fechamos e mudamos
            // cadeia de conexão para uma nova, se não -
            // recriamos os objetos de conexão e os comandos:
            if (conn != null)
            {
                conn.Close();
                conn.ConnectionString = sConnStr;
            }
            else
            {
                conn = new SqlConnection(sConnStr);
                com = new SqlCommand();
                com.Connection = conn;
            }
            // Tentamos abrir uma conexão:
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                // Por algum motivo, a conexão não foi aberta.
                // Colocamos as informações de erro na linha de mensagens:
                sMessage = ex.Message;
                // Liberamos recursos e redefinimos objetos:
                com.Dispose();
                conn.Dispose();
                conn = null;
                com = null;
                // Erro:
                return iResError;
            }
            // Tudo correu bem, a conexão está aberta:
            return iResSuccess;
        }


        [DllExport("GetLastMessage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static string GetLastMessage()
        {
            return sMessage;
        }

        [DllExport("ExecuteSql", CallingConvention = CallingConvention.StdCall)]
        public static int ExecuteSql(
        [MarshalAs(UnmanagedType.LPWStr)] string sSql)
        {
            // Limpamos a linha de mensagem:
            sMessage = string.Empty;
            // Primeiro precisamos verificar se a conexão está estabelecida.
            if (conn == null)
            {
                // A conexão ainda não está aberta.
                // Notificamos sobre o erro e retornamos o sinalizador do erro:
                sMessage = "Connection is null, call CreateConnection first.";
                return iResError;
            }
            // A conexão está pronta, tentamos executar o comando.
            try
            {
                com.CommandText = sSql;
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Erro ao executar o comando.
                // Colocamos as informações de erro na linha de mensagens:
                sMessage = ex.Message;
                // Retornamos o sinalizador de erro:
                return iResError;
            }
            // Tudo correu bem, portanto, devolvemos o sinalizador de execução bem-sucedida:
            return iResSuccess;
        }



        [DllExport("ReadInt", CallingConvention = CallingConvention.StdCall)]
        public static int ReadInt(
        [MarshalAs(UnmanagedType.LPWStr)] string sSql)
        {
            // Limpamos a linha de mensagem:
            sMessage = string.Empty;
            // Primeiro precisamos verificar se a conexão está estabelecida.
            if (conn == null)
            {
                // A conexão ainda não está aberta.
                // Notificamos sobre o erro e retornamos o sinalizador do erro:
                sMessage = "Connection is null, call CreateConnection first.";
                return iResError;
            }
            // Variável para obter o resultado de retorno:
            int iResult = 0;
            // A conexão está pronta, tentamos executar o comando.
            try
            {
                com.CommandText = sSql;
                iResult = (int)com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                // Erro ao executar o comando.
                // Colocamos as informações de erro na linha de mensagens:
                sMessage = ex.Message;
            }
            // Retornamos o resultado:
            return iResult;
        }


        [DllExport("ReadString", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static string ReadString(
        [MarshalAs(UnmanagedType.LPWStr)] string sSql)
        {
            // Limpamos a linha de mensagem:
            sMessage = string.Empty;
            // Primeiro precisamos verificar se a conexão está estabelecida.
            if (conn == null)
            {
                // A conexão ainda não está aberta.
                // Notificamos sobre o erro e retornamos o sinalizador do erro:
                sMessage = "Connection is null, call CreateConnection first.";
                return string.Empty;
            }
            // Variável para obter o resultado de retorno:
            string sResult = string.Empty;
            // A conexão está pronta, tentamos executar o comando.
            try
            {
                com.CommandText = sSql;
                sResult = com.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                // Erro ao executar o comando.
                // Colocamos as informações de erro na linha de mensagens:
                sMessage = ex.Message;
            }
            // Retornamos o resultado:
            return sResult;
        }


        [DllExport("CloseConnection", CallingConvention = CallingConvention.StdCall)]
        public static void CloseConnection()
        {
            // Primeiro precisamos verificar se a conexão está estabelecida.
            if (conn == null)
                // A conexão ainda não está aberta - significa que também não precisamos fechá-la:
                return;
            // Conexão está aberta - devemos fechá-la:
            com.Dispose();
            com = null;
            conn.Close();
            conn.Dispose();
            conn = null;
        }

    }
}
