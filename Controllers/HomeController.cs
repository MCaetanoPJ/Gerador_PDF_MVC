using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WebGerarPDF.Models;

namespace WebGerarPDF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public FileResult GerarRelatorio_Entrada()
        {
            //Aloca um bloco de mémoria para criar o arquivo PDF
            MemoryStream stream = new MemoryStream();
            using (var wri = new PdfWriter(stream))
            using (var pdfDoc = new PdfDocument(wri))
            using (var document = new Document(pdfDoc, new PageSize(PageSize.A4)))
            {
                #region Confiiguração da Página
                document.SetMargins(2, 2, 2, 2);
                int tamanhoFonte = 5;
                #endregion

                #region Configuracao da Tabela
                //Configuração da Tabela
                Table tableCabecalho = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }));//19 colunas
                tableCabecalho.SetWidth(UnitValue.CreatePercentValue(100));
                tableCabecalho.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                tableCabecalho.SetTextAlignment(TextAlignment.CENTER);
                #endregion

                var dadosLivroFiscal = new LivroFiscal
                {
                    RazaoSocial = "Nome da empresa...",
                    Cnpj = "00.000.000.00-.00"
                };
                List<LivroFiscal> listaLivroFiscal = new List<LivroFiscal>();
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);
                listaLivroFiscal.Add(dadosLivroFiscal);


                //Cabeçalho com dados da empresa
                DadosCabecalhoEmpresa(tamanhoFonte, tableCabecalho, dadosLivroFiscal);

                //Cabeçalho do Registro de Entradas
                DadosCabecalhoModeloEntrada(tamanhoFonte, tableCabecalho);

                //Registros
                int aux = 0;
                foreach (LivroFiscal livroFiscal in listaLivroFiscal)
                {
                    DadosRegistros(tamanhoFonte, tableCabecalho, livroFiscal);

                    //Insere o total por página a cada 56 registros
                    aux = aux + 1;
                    if (aux == 56)//ToDo: Cada página suporta no máx 59 linhas de registros
                    {
                        TituloPorTotalPagina(tamanhoFonte, tableCabecalho, "774.727,70", "1", "0,00", "0,00", "0,00", "0,00");
                        DadosTotalPagina(tamanhoFonte, tableCabecalho, "2", "1.210,00");
                        DadosTotalPagina(tamanhoFonte, tableCabecalho, "3", "667.289,68");
                        RodapeTotalPagina(tamanhoFonte, tableCabecalho, "4", "0,00", "0,00");
                        aux = 0;
                    }
                }

                //Insere o total de página caso tenha menos de 56 registros
                if (listaLivroFiscal.Count <= 56)
                {
                    TituloPorTotalPagina(tamanhoFonte, tableCabecalho, "774.727,70", "1", "0,00", "0,00", "0,00", "0,00");
                    DadosTotalPagina(tamanhoFonte, tableCabecalho, "2", "1.210,00");
                    DadosTotalPagina(tamanhoFonte, tableCabecalho, "3", "667.289,68");
                    RodapeTotalPagina(tamanhoFonte, tableCabecalho, "4", "0,00", "0,00");
                }

                //Demonstrativo por CFOP
                for (int i = 0; i < 9; i++)
                {
                    TituloDemonstrativoCFOP(tamanhoFonte, tableCabecalho, "566.817,93", "1-102", "1", "0,00", "0,00");
                    DadosDemonstrativoCFOP(tamanhoFonte, tableCabecalho, "2", "1.260,00");
                    DadosDemonstrativoCFOP(tamanhoFonte, tableCabecalho, "3", "565.557,93");
                    RodapeDemonstrativoCFOP(tamanhoFonte, tableCabecalho, "4", "0,00", "0,00");
                }

                //Total ICMS Mensal
                for (int i = 0; i < 1; i++)
                {
                    TituloICMSMensal(tamanhoFonte, tableCabecalho, "1.443.227,38", "1", "0,00", "0,00");
                    DadosICMSMensal(tamanhoFonte, tableCabecalho, "2", "4.360,00");
                    DadosICMSMensal(tamanhoFonte, tableCabecalho, "3", "1.438.867,38");
                    RodapeICMSMensal(tamanhoFonte, tableCabecalho, "4", "0,00", "0,00");
                }

                document.Add(tableCabecalho);
            }

            //Inicia o download do arquivo PDF
            var contantType = "application/pdf";
            var namePdfFile = "LivroFiscal_Entrada.pdf";
            return File(stream.ToArray(), contantType, namePdfFile);
        }

        //Cabeçalho da tabela
        private static void DadosCabecalhoEmpresa(int tamanhoFonte, Table tableCabecalho, LivroFiscal dadosEmpresa)
        {
            //Titulo do relatório
            tableCabecalho.AddHeaderCell(new Cell(0, 18).Add(new Paragraph("LIVRO REGISTRO DE ENTRADAS - RE - MODELO P1").SetFontSize(tamanhoFonte + 2)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));

            //Tabela com dados da empresa
            tableCabecalho.AddHeaderCell(new Cell(0, 10).Add(new Paragraph("REGISTRO DE ENTRADAS").SetFontSize(tamanhoFonte + 2)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER));
            tableCabecalho.AddHeaderCell(new Cell(0, 9).Add(new Paragraph("(*) CÓDIGO DE VALORES FISCAIS").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));

            tableCabecalho.AddHeaderCell(new Cell(0, 10).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            tableCabecalho.AddHeaderCell(new Cell(0, 9).Add(new Paragraph("1-OPERAÇÕES COM CRÉDITO DO IMPOSTO").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));

            tableCabecalho.AddHeaderCell(new Cell(0, 10).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            tableCabecalho.AddHeaderCell(new Cell(0, 9).Add(new Paragraph("2-OPER. SEM CRÉDITO DO IMPOSTO - ISENTAS/NÃO TRIBUTADAS").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));

            tableCabecalho.AddHeaderCell(new Cell(0, 10).Add(new Paragraph(string.Format("EMPRESA: {0}", dadosEmpresa.RazaoSocial)).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            tableCabecalho.AddHeaderCell(new Cell(0, 9).Add(new Paragraph("3-OPERAÇÕES SEM CRÉDITO DO IMPOSTO - OUTRAS").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));

            tableCabecalho.AddHeaderCell(new Cell(0, 4).Add(new Paragraph(string.Format("INSC.EST: {0}", dadosEmpresa.Inscricao)).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            tableCabecalho.AddHeaderCell(new Cell(0, 6).Add(new Paragraph(string.Format("CNPJ: {0}", dadosEmpresa.Cnpj)).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            tableCabecalho.AddHeaderCell(new Cell(0, 9).Add(new Paragraph("4-ICMS RETIDO POR SUBST.TRIBUTÁRIA").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));

            string numeroPaginas = "1".ToString().PadLeft(3, '0');
            tableCabecalho.AddHeaderCell(new Cell(0, 3).Add(new Paragraph(string.Format("FOLHA: {0}", numeroPaginas)).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            tableCabecalho.AddHeaderCell(new Cell(0, 1).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            tableCabecalho.AddHeaderCell(new Cell(0, 6).Add(new Paragraph(string.Format("MÊS OU PERÍODO/ANO: {0} a {1}", dadosEmpresa.DataInicial.Date.ToString("dd/MM/yyyy"), dadosEmpresa.DataFinal.ToString("dd/MM/yyyy"))).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            tableCabecalho.AddHeaderCell(new Cell(0, 9).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
        }
        private static void DadosCabecalhoModeloEntrada(int tamanhoFonte, Table tableCabecalho)
        {
            //Primeira Linha
            tableCabecalho.AddHeaderCell(new Cell(2, 0).Add(new Paragraph("DATA DE ENTRADA").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));
            tableCabecalho.AddHeaderCell(new Cell(0, 6).Add(new Paragraph("DOCUMENTOS FISCAIS").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));
            tableCabecalho.AddHeaderCell(new Cell(2, 0).Add(new Paragraph("VALOR CONTÁBIL").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));
            tableCabecalho.AddHeaderCell(new Cell(0, 2).Add(new Paragraph("CODIFICAÇÃO").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));
            tableCabecalho.AddHeaderCell(new Cell(0, 4).Add(new Paragraph("ICMS VALORES FISCAIS").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));
            tableCabecalho.AddHeaderCell(new Cell(0, 3).Add(new Paragraph("IPI VALORES FISCAIS").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));
            tableCabecalho.AddHeaderCell(new Cell(2, 0).Add(new Paragraph("OBSERVAÇÕES").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));

            //Segunda Linha
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("ESPÉCIE").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("SÉRIE/SUB SÉRIE").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("NÚMERO").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("DATA DO DOCUMENTO").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("CÓD. DO EMITENTE").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("UF ORIGEM").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("CONTÁBIL").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("FISCAL").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("COD (*)").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("BASE CÁLCULO VALOR OPERAÇÃO").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("ALIQ.%").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("IMPOSTO CREDITADO").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("COD (*)").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("BASE DE CÁLCULO VALOR DA OPERAÇÃO").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
            tableCabecalho.AddHeaderCell(new Cell(0, 0).Add(new Paragraph("IMPOSTO CREDITADO").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT));
        }


        //Conteúdo da tabela
        private static void DadosRegistros(int tamanhoFonte, Table tableCabecalho, LivroFiscal livroFiscal)
        {
            //Data de entrada
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "04/04/2021")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Especie
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", livroFiscal.Especie)).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Serie/Sub Serie
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", livroFiscal.Serie)).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Numero
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "995")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Data do Documento
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "05/04/2021")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Cod do Emitente
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "071")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //UF Origem
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", livroFiscal.EstadoEmissao)).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "2.000,00")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Cod (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "2-102")).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Base Calculo Valor Operacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "3")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Aliq
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "3.000,00")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Imposto Creditado
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Cod (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Base Calculo Valor Operacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Imposto Creditado
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Cod (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Observacoes
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(string.Format("{0}", "")).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
        }


        //Total por página
        private static void TituloPorTotalPagina(int tamanhoFonte, Table tableCabecalho, string totalDaPagina, string cod, string ICMS_baseCalculoValorOperacao, string ICMS_ImpostoCreditado, string IPI_baseCalculoValorOperacao, string IPI_ImpostoCreditado)
        {
            //Total da página
            tableCabecalho.AddCell(new Cell(0, 7).Add(new Paragraph("TOTAL DA PÁGINA").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(totalDaPagina).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(cod).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //BASE CÁLCULO VALOR OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(ICMS_baseCalculoValorOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //ALIQ.%
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(ICMS_ImpostoCreditado).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //BASE DE CÁLCULO VALOR DA OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(IPI_baseCalculoValorOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(IPI_ImpostoCreditado).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //Observacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
        }
        private static void DadosTotalPagina(int tamanhoFonte, Table tableCabecalho, string cod, string baseCalculoOperacao)
        {
            tableCabecalho.AddCell(new Cell(0, 7).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                        .SetBorderBottom(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetBorderTop(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(cod).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE CÁLCULO VALOR OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(baseCalculoOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //ALIQ.%
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE DE CÁLCULO VALOR DA OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Observacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
        }
        private static void RodapeTotalPagina(int tamanhoFonte, Table tableCabecalho, string cod, string ICMS_baseCalculoValorOperacao, string ICMS_ImpostoCreditado)
        {
            tableCabecalho.AddCell(new Cell(0, 7).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                        .SetBorderBottom(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetBorderTop(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(cod).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE CÁLCULO VALOR OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(ICMS_baseCalculoValorOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //ALIQ.%
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("0,00").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE DE CÁLCULO VALOR DA OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Observacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
        }


        //Demonstrativo CFOP
        private static void TituloDemonstrativoCFOP(int tamanhoFonte, Table tableCabecalho, string totalValorContabil, string fiscal, string iCMS_Cod, string iCMS_BaseCalculoValorOperacao, string iCMS_ImpostoCreditado)
        {
            //DEMONSTRATIVO POR CFOP
            tableCabecalho.AddCell(new Cell(0, 7).Add(new Paragraph("DEMONSTRATIVO POR CFOP").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(totalValorContabil).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(fiscal).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_Cod).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //BASE CÁLCULO VALOR OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_BaseCalculoValorOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //ALIQ.%
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_ImpostoCreditado).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //BASE DE CÁLCULO VALOR DA OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //Observacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
        }
        private static void DadosDemonstrativoCFOP(int tamanhoFonte, Table tableCabecalho, string iCMS_Cod, string iCMS_BaseCalculoValorOperacao)
        {
            tableCabecalho.AddCell(new Cell(0, 7).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_Cod).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE CÁLCULO VALOR OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_BaseCalculoValorOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //ALIQ.%
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE DE CÁLCULO VALOR DA OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Observacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
        }
        private static void RodapeDemonstrativoCFOP(int tamanhoFonte, Table tableCabecalho, string iCMS_Cod, string iCMS_BaseCalculoValorOperacao, string iCMS_ImpostoCreditado)
        {
            tableCabecalho.AddCell(new Cell(0, 7).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("0,00 ").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_Cod).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE CÁLCULO VALOR OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_BaseCalculoValorOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //ALIQ.%
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_ImpostoCreditado).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE DE CÁLCULO VALOR DA OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Observacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
        }


        //Cálculo do ICMS Mensal
        private static void TituloICMSMensal(int tamanhoFonte, Table tableCabecalho, string totalValorContabil, string iCMS_Cod, string iCMS_BaseCalculoValorOperacao, string iCMS_ImpostoCreditado)
        {
            //TOTAL ICMS MENSAL
            tableCabecalho.AddCell(new Cell(0, 7).Add(new Paragraph("TOTAL ICMS MENSAL").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(totalValorContabil).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_Cod).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //BASE CÁLCULO VALOR OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_BaseCalculoValorOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //ALIQ.%
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_ImpostoCreditado).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //BASE DE CÁLCULO VALOR DA OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
            //Observacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER));
        }
        private static void DadosICMSMensal(int tamanhoFonte, Table tableCabecalho, string iCMS_Cod, string iCMS_BaseCalculoValorOperacao)
        {
            tableCabecalho.AddCell(new Cell(0, 7).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_Cod).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE CÁLCULO VALOR OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_BaseCalculoValorOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //ALIQ.%
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE DE CÁLCULO VALOR DA OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Observacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
        }
        private static void RodapeICMSMensal(int tamanhoFonte, Table tableCabecalho, string iCMS_Cod, string iCMS_BaseCalculoValorOperacao, string iCMS_ImpostoCreditado)
        {
            tableCabecalho.AddCell(new Cell(0, 7).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Valor Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Contabil
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Fiscal
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.CENTER)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_Cod).SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE CÁLCULO VALOR OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_BaseCalculoValorOperacao).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //ALIQ.%
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph(iCMS_ImpostoCreditado).SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //COD (*)
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetMaxWidth(1).SetTextAlignment(TextAlignment.LEFT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //BASE DE CÁLCULO VALOR DA OPERAÇÃO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //IMPOSTO CREDITADO
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderRight(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
            //Observacao
            tableCabecalho.AddCell(new Cell(0, 0).Add(new Paragraph("").SetFontSize(tamanhoFonte)).SetTextAlignment(TextAlignment.RIGHT)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER));
        }
    }

    public class LivroFiscalRodape
    {
        public double TotalCfopTrib { get; set; }
        public double TotalCfopNaoTrib { get; set; }
        public double Cfop { get; set; }
        public double TotalBase { get; set; }
        public double TotalLig { get; set; }
        public double Outros { get; set; }
    }
    public class LivroFiscal //: Entity
    {
        public int Dia { get; set; }
        public string RazaoSocial { get; set; }
        public string Especie { get; set; }
        public string Cnpj { get; set; }
        public string Inscricao { get; set; }
        public string EstadoEmissao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime DataFinal { get; set; }
        public string Operacao { get; set; }
        public int N_NotaFiscal { get; set; }
        public string Serie { get; set; }
        public double TotalNfeTributado { get; set; }
        public double TotalNfeNaoTributado { get; set; }
        public double CFOP { get; set; }
        public double BASECALCULO { get; set; }
        public double ALIQ { get; set; }
        public double IMPDEBITADO { get; set; }
        public double OUTRAS { get; set; }
        //public List<Items> ListProdutos { get; set; }
    }
}
