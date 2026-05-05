# NetworkMessage

Unity 2022.3 · Mirror · Zenject · MVP

## Структура проекта

```
Assets/Scripts/NetworkMirror/
├── Runtime/
│   ├── Messages/
│   │   ├── NetworkEnvelopeMessage.cs
│   │   ├── SubscribeRequestMessage.cs
│   │   └── UnsubscribeRequestMessage.cs
│   ├── Service/
│   │   ├── INetworkMessageService.cs
│   │   └── NetworkMessageService.cs
│   ├── Presenter/
│   │   └── NetworkMessagePresenter.cs
│   ├── View/
│   │   ├── INetworkMessageView.cs
│   │   └── NetworkMessageView.cs
│   └── AppNetworkManager.cs
├── Demo/
│   ├── HelloMessage.cs
│   ├── DemoServer.cs
│   └── AutoHost.cs
└── Installer/
    └── NetworkMessageInstaller.cs
```

## Архитектура

### Проблема Mirror

Mirror разрывает соединение с клиентом, если сервер отправляет тип сообщения, для которого клиент не зарегистрировал обработчик.

### Решение — паттерн «конверт»

Клиент регистрирует один обработчик — `NetworkEnvelopeMessage`. Реальное сообщение сериализуется в `byte[]` и упаковывается внутрь конверта. Сервер отправляет конверт только тем клиентам, которые явно подписались.

```
Client                                Server
  │─ SubscribeRequestMessage ────────▶│
  │                                   │ subscribers[typeId].Add(conn)
  │◀─ NetworkEnvelopeMessage ─────────│ только подписанным
       { MessageTypeId, Payload }
```

### Поток данных

```
AppNetworkManager.OnClientConnect()
  └─ NetworkMessagePresenter.OnClientConnected()
       └─ INetworkMessageService.ClientSubscribe<HelloMessage>()
            └─ NetworkClient.Send(SubscribeRequestMessage)

NetworkServer → OnServerReceiveSubscribe()
  └─ OnClientSubscribed event
       └─ DemoServer → INetworkMessageService.ServerSend(conn, HelloMessage)
            └─ writer.Write(message) → NetworkEnvelopeMessage → conn.Send()

NetworkClient → OnClientReceiveEnvelope()
  └─ reader.Read<HelloMessage>()
       └─ NetworkMessagePresenter → INetworkMessageView.ShowMessage()
```

## Зависимости

| Пакет   | Расположение     |
|---------|------------------|
| Mirror  | Assets/Mirror/   |
| Zenject | Assets/Plugins/  |
