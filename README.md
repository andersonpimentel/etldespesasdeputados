# ETL de Despesas dos Deputados

Nosso objetivo final neste trabalho é analisar como os deputados brasileiros, a nível estadual e federal, estão utilizando a cota parlamentar que possuem para gastos reembolsáveis durante o exercício da vida pública.

Cada parlamentar brasileiro dispõe de uma cota mensal, que varia para cada Estado, entre um mínimo de R$ 30.788,66 (DF) e um máximo de R$ 45.612,53 (RR). Dentro deste limite, cada deputado pode pedir o reembolso de gastos que entram em determinadas categorias – como locomoção, alimentação, hospedagem, aluguel, segurança, entre outros.

## Base de dados

Utilizamos uma base de dados do brasil.IO, site que disponibiliza dados liberados por agências públicas brasileiras por repositórios e
APIs.

- Base de dados: [Dataset completo](https://drive.google.com/open?id=13IJ1iAScmmUWTk-pgogVwJq1b6jTKpyH)
- Fonte original: [Câmara dos Deputados do Brasil](http://www2.camara.leg.br/transparencia/cota-para-exercicio-da-atividade-parlamentar/dados-abertos-cota-parlamentar)
- Disponibilizado por: [brasil.IO](https://brasil.io/dataset/gastos-deputados/cota_parlamentar)
- Liberado por: [Álvaro Justen](https://twitter.com/turicas)
- Código-fonte: https://github.com/turicas/gastos-deputados
- Licença: [Creative Commons Attribution-ShareAlike 4.0 International](https://creativecommons.org/licenses/by-sa/4.0/)
- Formato do arquivo: [Dicionário de dados](http://www2.camara.leg.br/transparencia/cota-para-exercicio-da-atividade-parlamentar/explicacoes-sobre-o-formato-dos-arquivos-xml)

O dataset completo possui um total de 3.544.400 registros entre os anos de 2009 e 2019 (incompleto), referentes a gastos reembolsados
por deputados estaduais e federais por meio da cota parlamentar.

## Código fonte

Disponibilizamos o script da criação do banco de dados (em SQL), o ETL realizado pelo Pentaho, uma versão alternativa do ETL (em C#) e
um notebook com que geramos o data frame inicial utilizando Pandas.
