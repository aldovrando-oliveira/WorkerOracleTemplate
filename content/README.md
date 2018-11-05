# PROJECT-TITLE

PROJECT-DESCRIPTION

## Sumário

- [Configurações](#markdown-header-configuracoes)  
  - [Banco de Dados](#markdown-header-banco-de-dados)

## Configurações

### Banco de Dados

A configuração com o banco de dados deve ser feita utilizando as variáveis de ambiente abaixo:
```
DB_DATASOURCE = Texto com as configurações para conexão com o banco
DB_USERNAME = Texto com o usuário de conexão ao banco
DB_PASSWORD = Texto com a senha de conexão ao banco
DB_POOLING = [true | false] (Padrão: false)
DB_POOLING_MIN_SIZE = Valor numérico indicando o tamanho minimo do pool de conexão (Padrão: 3)
DB_POOLING_MAX_SIZE = Valor numérico indicando o tamanho máximo do pool de conexão (Padrão: 5)
```
> Quando a variável `DB_POOLING` não é definida, ou é definida como `false` as variáveis `DB_POOLING_MIN_SIZE` e `DB_POOLING_MIN_SIZE` são desconsideradas