﻿/* 
 * nMQTT, a .Net MQTT v3 client implementation.
 * http://code.google.com/p/nmqtt
 * 
 * Copyright (c) 2009 Mark Allanson (mark@markallanson.net)
 *
 * Licensed under the MIT License. You may not use this file except 
 * in compliance with the License. You may obtain a copy of the License at
 *
 *     http://www.opensource.org/licenses/mit-license.php
*/

//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.IO;

namespace Nmqtt
{
    /// <summary>
    /// A client class for interacting with MQTT Data Packets
    /// </summary>
    public sealed class MqttClient : IDisposable
    {
        private string server;
        /// <summary>
        /// The remote server that this client will connect to.
        /// </summary>
        public string Server
        {
            get
            {
                return server;
            }
        }

        private int port;
        /// <summary>
        /// The port on the remote server that this client will connect to.
        /// </summary>
        public int Port
        {
            get
            {
                return port;
            }
        }

        private string clientIdentifier;
        /// <summary>
        /// Gets the Client Identifier of this instance of the MqttClient
        /// </summary>
        public string ClientIdentifier
        {
            get
            {
                return clientIdentifier;
            }
        }

        /// <summary>
        /// Gets the current conneciton state of the Mqtt Client.
        /// </summary>
        public ConnectionState ConnectionState
        {
            get
            {
                if (connectionHandler != null)
                {
                    return connectionHandler.State;
                }
                else
                {
                    return Nmqtt.ConnectionState.Disconnected;
                }
            }
        }

        /// <summary>
        /// The Handler that is managing the connection to the remote server.
        /// </summary>
        private MqttConnectionHandler connectionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttClient"/> class using the default Mqtt Port.
        /// </summary>
        /// <param name="server">The server hostname to connect to.</param>
        /// <param name="clientIdentifier">The client identifier to use to connect with.</param>
        public MqttClient(string server, string clientIdentifier)
            : this(server, Constants.DefaultMqttPort, clientIdentifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttClient"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="port">The port.</param>
        /// <param name="clientIdentifier">The ID that the broker can use to identify the client.</param>
        public MqttClient(string server, int port, string clientIdentifier)
        {
            this.server = server;
            this.port = port;
            this.clientIdentifier = clientIdentifier;
        }

        /// <summary>
        /// Performs a synchronous connect to the message broker.
        /// </summary>
        public ConnectionState Connect()
        {
            MqttConnectMessage connectMessage = GetConnectMessage();
            connectionHandler = new SynchronousMqttConnectionHandler();
            connectionHandler.MessageReceived += connectionHandler_MessageReceived;
            return connectionHandler.Connect(this.server, this.port, connectMessage);
        }

        /// <summary>
        /// Initiates an Asynchronous connection to the message broker. The ConnectComplete 
        /// event is fired when connection has finished.
        /// </summary>
        //public void BeginConnect()
        //{
        //    MqttConnectMessage connectMessage = GetConnectMessage();
        //}

        /// <summary>
        /// Gets a configured connect message.
        /// </summary>
        /// <returns>An MqttConnectMessage that can be used to connect to a message broker.</returns>
        private MqttConnectMessage GetConnectMessage()
        {
            return new MqttConnectMessage()
                .WithClientIdentifier(clientIdentifier)
                .KeepAliveFor(30)
                .StartClean();

        }

        /// <summary>
        /// The primary message processor that deals with incoming messages from thr Mqtt Connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectionHandler_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                // TODO: Decide whether we should spawn our message processing on a new thread.
                HandleReceivedMessage(e.Message);
            }
            catch (InvalidMessageException ex)
            {
                if (InvalidMessageReceived != null)
                {
                    // TODO: Implement the Invalid Message Event Args properly.
                    InvalidMessageReceived(this, new InvalidMessageEventArgs(ex));
                }
            }
        }

        /// <summary>
        /// Handles the processing of messages arriving from the message broker.
        /// </summary>
        /// <param name="mqttMessage"></param>
        private void HandleReceivedMessage(MqttMessage message)
        {
            // handle the basic publish message by firing the PublishMessageReceived event of the client
            // for application level handling of message data.
            if (message.Header.MessageType == MqttMessageType.Publish)
            {
                MqttPublishMessage published = (MqttPublishMessage)message;
                if (PublishMessageReceived != null)
                {
                    PublishMessageReceived(this, new PublishEventArgs(published.Payload.Message));
                }
            }
        }

        /// <summary>
        /// Event fired when the connect to a remove server has been completed.
        /// </summary>
        //public event EventHandler<EventArgs> ConnectComplete;

        /// <summary>
        /// Event fired when a publish message has been received by the client. 
        /// </summary>
        public event EventHandler<PublishEventArgs> PublishMessageReceived;

        /// <summary>
        /// Event fired when a message received from the connect message broker could not be processed.
        /// </summary>
        public event EventHandler<InvalidMessageEventArgs> InvalidMessageReceived;

        /// <summary>
        /// Closes the MQTT Client.
        /// </summary>
        public void Close()
        {
            connectionHandler.Close();
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Close();
            connectionHandler.Dispose();
        }

        #endregion
    }
}
