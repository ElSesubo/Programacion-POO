﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace AEV7
{
    class Registro
    {
        private int id;
        private string nif;
        private DateTime fecha;
        private string fichajeEntrada;
        private string fichajeSalida;
        private bool finalizado;

        public int Id { get { return id; } set { id = value;} }
        public string Nif { get { return nif; } set { nif = value; } }
        public DateTime Fecha { get { return fecha; } set { fecha = value; } }
        public string FichajeEntrada { get { return fichajeEntrada; } set { fichajeEntrada = value; } }
        public string FichajeSalida { get { return fichajeSalida; } set { fichajeSalida = value; } }
        public bool Finalizado { get { return finalizado; } set { finalizado = value; } }

        public Registro(int i, string n, DateTime f, string fEntrada, string fSalida, bool fin)
        {
            id = i;
            nif = n;
            fecha = f;
            fichajeEntrada = fEntrada;
            fichajeSalida = fSalida;
            finalizado = fin;
        }

        public Registro()
        {

        }

        public int ficharEntrada(MySqlConnection conexion, string nif, string hora)
        {
            int retorno;
            string consulta = string.Format("INSERT INTO fichaje (NIF,fecha,fichajeEntrada) VALUES('{0}',@fecha,'{1}')", nif, hora);

            MySqlCommand comando = new MySqlCommand(consulta, conexion);
            comando.Parameters.AddWithValue("fecha", DateTime.Now.ToString("yyyyMMdd"));

            retorno = comando.ExecuteNonQuery();
            return retorno;
        }

        public int ficharSalida(MySqlConnection conexion, string hora, string nif)
        {
            int retorno;
            string consulta = string.Format("UPDATE fichaje SET fichajeSalida='{0}',finalizado=true WHERE nif='{1}' AND finalizado=0", hora, nif);

            MySqlCommand comando = new MySqlCommand(consulta, conexion);

            retorno = comando.ExecuteNonQuery();
            return retorno;
        }


        public static List<Registro> BuscarUsuario(MySqlConnection conexion, string consulta)
        {
            List<Registro> lista = new List<Registro>();

            MySqlCommand comando = new MySqlCommand(consulta, conexion);
            MySqlDataReader reader = comando.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Registro user = new Registro(reader.GetInt32(0),reader.GetString(1),reader.GetDateTime(2),reader.GetString(3),reader.GetString(4),reader.GetBoolean(5));
                        lista.Add(user);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            return lista;
        }

        static public List<Registro> permanencia(MySqlConnection conexion, string nif, DateTime inicio, DateTime fin)
        {
            List<Registro> fichajes = new List<Registro>();
            string fechaIni = inicio.ToString("yyyyMMdd");
            string fechaSal = fin.ToString("yyyyMMdd");
            string consulta = String.Format("SELECT * FROM fichaje WHERE nif='{0}' AND (fecha BETWEEN {1} and {2}) AND finalizado=TRUE;", nif.ToUpper(), fechaIni, fechaSal);

            MySqlCommand comando = new MySqlCommand(consulta, conexion);
            MySqlDataReader reader = comando.ExecuteReader();
            while (reader.Read())
            {
                fichajes.Add(new Registro(reader.GetInt32(0),reader.GetString(1),reader.GetDateTime(2), reader.GetString(3), reader.GetString(4),reader.GetBoolean(5)));
            }
            reader.Close();
            return fichajes;
        }
    }
}
