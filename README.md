
# 📬 MailClient - ASP.NET Core Project

Projeto para leitura de e-mails a partir de uma caixa de entrada configurada, utilizando uma API ASP.NET Core e uma aplicação console (consumer).  
A API se autentica com o servidor de e-mails, busca mensagens em um período definido e as envia para uma fila no **RabbitMQ**.  
O **Consumer** então processa essas mensagens e as salva em um banco de dados **MongoDB**.

---

## ✅ Requisitos

- [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- MongoDB
- RabbitMQ

---

## 🚀 Opções de Execução

### 1. API - Aplicação Web ASP.NET Core

A API serve como ponto de entrada para buscar os e-mails da caixa de entrada configurada.

### 2. Aplicação Console (Consumer)

Responsável por consumir as mensagens da fila do RabbitMQ e persistir no MongoDB.

---

## 🔧 Build e Execução

Acesse o diretório raiz do projeto e execute os seguintes comandos:

### 📡 Executar a API:

```bash
dotnet build
dotnet run .\MailClient.API.csproj
```

### 🖥️ Executar o Consumer:

```bash
dotnet run .\MailClient.Consumer.csproj
```

---

## 🐳 Subindo MongoDB e RabbitMQ com Docker

Execute os comandos abaixo para rodar MongoDB e RabbitMQ em containers:

```bash
docker volume create mongodb

docker run --name mongodb -d -p 27017:27017 -v mongodb:/data/db mongo

docker run --name rabbitmq -d \
  -p 15672:15672 \
  -p 5672:5672 \
  -p 25676:25676 \
  rabbitmq:3-management
```

---

## 📄 Licença

Este projeto pode ser licenciado conforme sua preferência. Adicione o arquivo `LICENSE` com os detalhes da licença desejada.

---

## 👨‍💻 Desenvolvido por

Tiago (MailClient)

---
