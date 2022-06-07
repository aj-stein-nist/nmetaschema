/*
 * Portions of this software was developed by employees of the National Institute
 * of Standards and Technology (NIST), an agency of the Federal Government and is
 * being made available as a public service. Pursuant to title 17 United States
 * Code Section 105, works of NIST employees are not subject to copyright
 * protection in the United States. This software may be subject to foreign
 * copyright. Permission in the United States and in foreign countries, to the
 * extent that NIST may hold copyright, to use, copy, modify, create derivative
 * works, and distribute this software and its documentation without fee is hereby
 * granted on a non-exclusive basis, provided that this notice and disclaimer
 * of warranty appears in all copies.
 *
 * THE SOFTWARE IS PROVIDED 'AS IS' WITHOUT ANY WARRANTY OF ANY KIND, EITHER
 * EXPRESSED, IMPLIED, OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, ANY WARRANTY
 * THAT THE SOFTWARE WILL CONFORM TO SPECIFICATIONS, ANY IMPLIED WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AND FREEDOM FROM
 * INFRINGEMENT, AND ANY WARRANTY THAT THE DOCUMENTATION WILL CONFORM TO THE
 * SOFTWARE, OR ANY WARRANTY THAT THE SOFTWARE WILL BE ERROR FREE.  IN NO EVENT
 * SHALL NIST BE LIABLE FOR ANY DAMAGES, INCLUDING, BUT NOT LIMITED TO, DIRECT,
 * INDIRECT, SPECIAL OR CONSEQUENTIAL DAMAGES, ARISING OUT OF, RESULTING FROM,
 * OR IN ANY WAY CONNECTED WITH THIS SOFTWARE, WHETHER OR NOT BASED UPON WARRANTY,
 * CONTRACT, TORT, OR OTHERWISE, WHETHER OR NOT INJURY WAS SUSTAINED BY PERSONS OR
 * PROPERTY OR OTHERWISE, AND WHETHER OR NOT LOSS WAS SUSTAINED FROM, OR AROSE OUT
 * OF THE RESULTS OF, OR USE OF, THE SOFTWARE OR SERVICES PROVIDED HEREUNDER.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading;

namespace NMetaschema.Model.Common
{
    /// <summary>
    /// This is the abstract base class to instantiate one or more
    /// Metaschema definitions. <see href="https://pages.nist.gov/metaschema/specification/syntax/"/>
    /// </summary>
    public abstract class AbstractMetaschema : IMetaschema
    {
        private IList<IMetaschema> _importedMetaschemas;
        private Lazy<Exports> _exports;

        public AbstractMetaschema(IList<IMetaschema> importedMetaschemas){
            _importedMetaschemas = new ReadOnlyCollection<IMetaschema>(importedMetaschemas);
            _exports = new Lazy<Exports>(() => new Exports(this));
        }

        public static Expression<Func<IDefinition,bool>> keepAllNonLocalDefinitions
            = definition => ModuleScope.INHERITED.Equals(definition.getModuleScope())
                || ModelType.ASSEMBLY.Equals(definition.getModelType) && (definition as IAssemblyDefinition).isRoot();

        public abstract Uri getLocation();

        public abstract void getName();

        public abstract string getVersion();

        public abstract void getRemarks();

        public abstract string getShortName();

        public abstract Uri getXmlNamespace();

        public abstract Uri getJsonBaseUri();

        public IList<IMetaschema> getImportedMetaschemas() {
            return _importedMetaschemas;
        }

        protected Dictionary<String, IMetaschema> getImportedMetaschemaByShortNames() {
            return getImportedMetaschemas().ToDictionary(metaschema => metaschema.getShortName());
        }

        public IMetaschema? getImportedMetaschemaByShortName(string shortName) {
            IMetaschema value;
            if (getImportedMetaschemaByShortNames().TryGetValue(shortName, out value)) {
                return value;
            }
            return null;
        }

        public abstract IList<IAssemblyDefinition> getAssemblyDefinitions();

        public abstract IAssemblyDefinition? getAssemblyDefinitionByName(string name);

        public abstract IList<IFieldDefinition> getFieldDefinitions();

        public abstract IFieldDefinition? getFieldDefinitionByName(string name);

        public abstract IList<INamedModelDefinition> getAssemblyAndFieldDefinitions();

        public abstract IList<IFlagDefinition> getFlagDefinitions();

        public abstract IFlagDefinition? getFlagDefinitionByName(string name);
        
        public abstract IAssemblyDefinition? getScopedAssemblyDefinitionByName(string name);
        
        public abstract IFieldDefinition? getScopedFieldDefinitionByName(string name);
        
        public abstract IFlagDefinition? getScopedFlagDefinitionByName(string name);

        public abstract IList<IAssemblyDefinition> getRootAssemblyDefinitions();

        protected Dictionary<String, IFlagDefinition> getExportedFlagDefinitionMap() {
            return _exports.Value.FlagDefinitions;
        }

        public ICollection<IFlagDefinition> getExportedFlagDefinitions() {
            return getExportedFlagDefinitionMap().Values;
        }

        public IFlagDefinition? getExportedFlagDefinitionByName(string name) {
            IFlagDefinition value;
            if (getExportedFlagDefinitionMap().TryGetValue(name, out value)) {
                return value;
            }
            return null;
        }

        protected Dictionary<String, IFieldDefinition> getExportedFieldDefinitionMap() {
            return _exports.Value.FieldDefinitions;
        }

        public ICollection<IFieldDefinition> getExportedFieldDefinitions() {
            return getExportedFieldDefinitionMap().Values;
        }

        public IFieldDefinition? getExportedFieldDefinitionByName(string name) {
            IFieldDefinition value;
            if (getExportedFieldDefinitionMap().TryGetValue(name, out value)) {
                return value;
            }
            return null;
        }

        protected Dictionary<String, IAssemblyDefinition> getExportedAssemblyDefinitionMap() {
            return _exports.Value.AssemblyDefinitions;
        }

        public ICollection<IAssemblyDefinition> getExportedAssemblyDefinitions() {
            return getExportedAssemblyDefinitionMap().Values;
        }

        public IAssemblyDefinition? getExportedAssemblyDefinitionByName(string name) {
            IAssemblyDefinition value;
            if (getExportedAssemblyDefinitionMap().TryGetValue(name, out value)) {
                return value;
            }
            return null;
        }

        // TODO: Metapath will be implemented after core feature set.
        // void getInfoElementsByMetapath();

        private class Exports {
            private AbstractMetaschema parentInstance;

            private Dictionary<String, IFlagDefinition> _exportedFlagDefinitions;
            private Dictionary<String, IFieldDefinition> _exportedFieldDefinitions;
            private Dictionary<String, IAssemblyDefinition> _exportedAssemblyDefinitions;

            public Exports(AbstractMetaschema parentInstance) {
                _exportedFlagDefinitions = parentInstance.getFlagDefinitions().TakeWhile(keepAllNonLocalDefinitions);
                _exportedFieldDefinitions = parentInstance.getFieldDefinitions().TakeWhile(keepAllNonLocalDefinitions);
                _exportedAssemblyDefinitions = parentInstance.getFieldDefinitions().TakeWhile(keepAllNonLocalDefinitions);
            }

            public Dictionary<String, IFlagDefinition> FlagDefinitions {
                get {
                    return _exportedFlagDefinitions;
                }
            }

            public Dictionary<String, IFieldDefinition> FieldDefinitions {
                get {
                    return _exportedFieldDefinitions;
                }
            }

            public Dictionary<String, IAssemblyDefinition> AssemblyDefinitions {
                get {
                    return _exportedAssemblyDefinitions;
                }
            }
        }
    }
}
