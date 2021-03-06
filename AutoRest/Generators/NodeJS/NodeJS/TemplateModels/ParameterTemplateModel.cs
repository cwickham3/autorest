﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Rest.Generator.ClientModel;
using Microsoft.Rest.Generator.Utilities;

namespace Microsoft.Rest.Generator.NodeJS
{
    public class ParameterTemplateModel : Parameter
    {
        public ParameterTemplateModel(Parameter source)
        {
            this.LoadFrom(source);
        }

        /// <summary>
        /// Gets parameter declaration
        /// </summary>
        public virtual string DeclarationExpression
        {
            get { return this.Type.Name; }
        }
    }
}