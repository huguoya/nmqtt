/* 
 * nMQTT, a .Net MQTT v3 client implementation.
 * http://wiki.github.com/markallanson/nmqtt
 * 
 * Copyright (c) 2009 Mark Allanson (mark@markallanson.net)
 *
 * Licensed under the MIT License. You may not use this file except 
 * in compliance with the License. You may obtain a copy of the License at
 *
 *     http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nmqtt
{
    /// <summary>
    /// Describes the status of a subscription
    /// </summary>
    public enum SubscriptionStatus
    {

        /// <summary>
        /// The subscription does not exist / is not known
        /// </summary>
        DoesNotExist,

        /// <summary>
        /// The subscription is currently pending acknowledgement by a broker.
        /// </summary>
        Pending,

        /// <summary>
        /// The subscription is currently active and messages will be received.
        /// </summary>
        Active
    }
}
