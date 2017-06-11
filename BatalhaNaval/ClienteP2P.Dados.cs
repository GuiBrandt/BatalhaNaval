using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BatalhaNaval
{
    /// <summary>
    /// Cliente Peer-to-Peer para a batalha naval
    /// </summary>
    public sealed partial class ClienteP2P
    {
        /// <summary>
        /// Timeout para dar um tiro
        /// </summary>
        const int TIMEOUT_TIRO = 30000;

        /// <summary>
        /// Delegado para a função de controle de eventos de tiro recebido
        /// </summary>
        /// <param name="t">Objeto representando o tiro recebido</param>
        public delegate void EventoDeTiroRecebido(Tiro t);

        /// <summary>
        /// Delegado para função de controle de evento de quando deve-se dar um tiro
        /// </summary>
        /// <returns>Um objeto do tipo tiro, com uma coordenada X e Y</returns>
        public delegate void EventoDeDarTiro();

        /// <summary>
        /// Delegado para função de controle de evento de quando se recebe o resultado
        /// de um tiro dado
        /// </summary>
        /// <param name="t">Objeto representando o tiro recebido</param>
        /// <param name="resultado">Resultado do tiro</param>
        public delegate void EventoDeResultadoDeTiro(Tiro t, ResultadoDeTiro resultado);

        /// <summary>
        /// Evento chamado quando é seu turno de atirar
        /// </summary>
        public event EventoDeDarTiro OnDarTiro;

        /// <summary>
        /// Evento chamado quando recebe-se o resultado do último tiro dado
        /// </summary>
        public event EventoDeResultadoDeTiro OnResultadoDeTiro;

        /// <summary>
        /// Evento de tiro recebido
        /// </summary>
        public event EventoDeTiroRecebido OnTiroRecebido;

        /// <summary>
        /// Mapa usado pelo cliente
        /// </summary>
        public Tabuleiro Tabuleiro { get; private set; }

        /// <summary>
        /// Lista de tiros dados pelo cliente e seus resultados
        /// </summary>
        public ListaDeTiros TirosDados { get; private set; }

        /// <summary>
        /// Lista de todos os tiros recebidos pelo cliente
        /// </summary>
        public ListaDeTiros TirosRecebidos { get; private set; }

        /// <summary>
        /// Mutex para esperar um tiro
        /// </summary>
        private AutoResetEvent waitHandle;

        /// <summary>
        /// Tiro a ser dado
        /// </summary>
        private Tiro _tiro;

        /// <summary>
        /// Aleatório
        /// </summary>
        private Random rnd;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="nome">Nome do jogador, passado para os clientes remotos</param>
        /// <param name="tabuleiro">Tabuleiro com o qual se quer jogar</param>
        /// <exception cref="Exception">Se o tabuleiro estiver incompleto</exception>
        public ClienteP2P(string nome, Tabuleiro tabuleiro) : this(nome)
        {
            if (!tabuleiro.EstaCompleto())
                throw new Exception("Tabuleiro incompleto");

            rnd = new Random();
            waitHandle = new AutoResetEvent(false);
            TirosDados = new ListaDeTiros();
            TirosRecebidos = new ListaDeTiros();
            Tabuleiro = tabuleiro;
            OnClienteConectado += Dados_OnClienteConectado;
            OnClienteDesconectado += Dados_OnClienteDesconectado;
        }

        /// <summary>
        /// Evento de desconexão
        /// </summary>
        private void Dados_OnClienteDesconectado(IPAddress addr)
        {
            if (addr.Equals((cliente.Client.RemoteEndPoint as IPEndPoint).Address))
                Conectado = false;
        }

        /// <summary>
        /// Evento de sucesso de conexão com cliente
        /// </summary>
        /// <param name="addr">Endereço do cliente</param>
        private void Dados_OnClienteConectado(IPAddress addr)
        {
            Task.Run(() => Jogar());
        }

        /// <summary>
        /// Envia um tiro para o cliente
        /// </summary>
        /// <param name="x">Posição X do tiro</param>
        /// <param name="y">Posição Y do tiro</param>
        public void DarTiro(int x, int y)
        {
            _tiro = new Tiro(x, y);

            waitHandle.Set();
        }

        /// <summary>
        /// Executa o jogo se comunicando com o par remoto
        /// </summary>
        private void Jogar()
        {
            StreamWriter writer = new StreamWriter(cliente.GetStream());
            writer.AutoFlush = true;

            StreamReader reader = new StreamReader(cliente.GetStream());

            try
            {
                while (Conectado)
                {
                    Debugger.Log(0, "msg", "Sua vez" + Environment.NewLine);
                    OnDarTiro();
                    waitHandle.WaitOne(TIMEOUT_TIRO);

                    if (_tiro == null)
                        _tiro = new Tiro(rnd.Next(Tabuleiro.NumeroDeColunas), rnd.Next(Tabuleiro.NumeroDeLinhas));

                    writer.WriteLine("Tiro " + _tiro.X + "," + _tiro.Y);
                    Debugger.Log(0, "msg", "Tiro " + _tiro.X + "," + _tiro.Y + Environment.NewLine);

                    string r = reader.ReadLine();
                    Tiro recebido = null;

                    if (r.StartsWith("Tiro "))
                    {
                        int x = Convert.ToInt32(r.Substring(5, r.IndexOf(',') - 5));
                        int y = Convert.ToInt32(r.Substring(r.IndexOf(',') + 1));

                        Debugger.Log(0, "msg", "I '" + r + "'" + Environment.NewLine);
                        recebido = new Tiro(x, y);

                        ResultadoDeTiro resultado = recebido.Aplicar(Tabuleiro);
                        TirosRecebidos.Add(recebido, resultado);
                        Task.Run(() => OnTiroRecebido(recebido));

                        lock (writer)
                            writer.WriteLine(((uint)resultado).ToString());
                    }

                    while (!char.IsNumber(r[0])) r = reader.ReadLine();

                    ResultadoDeTiro result = (ResultadoDeTiro)Convert.ToUInt32(r);
                    TirosDados.Add(_tiro, result);
                    Task.Run(() => OnResultadoDeTiro(_tiro, result));

                    _tiro = null;

                    if (recebido == null)
                    {
                        string line;

                        line = reader.ReadLine();
                        Debugger.Log(0, "msg", "I '" + line + "'" + Environment.NewLine);
                        if (line.StartsWith("Tiro "))
                        {
                            int x = Convert.ToInt32(line.Substring(5, line.IndexOf(',') - 5));
                            int y = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));

                            recebido = new Tiro(x, y);
                            ResultadoDeTiro resultado = recebido.Aplicar(Tabuleiro);
                            TirosRecebidos.Add(recebido, resultado);
                            Task.Run(() => OnTiroRecebido(recebido));
                            
                            lock (writer)
                                writer.WriteLine(((uint)resultado).ToString());
                        }
                    }

                    waitHandle.Reset();
                }
            }
            catch (Exception e)
            {
                Debugger.Log(0, "error", e.Message + Environment.NewLine);
                OnClienteDesconectado((cliente.Client.RemoteEndPoint as IPEndPoint).Address);
            }
        }
    }
}