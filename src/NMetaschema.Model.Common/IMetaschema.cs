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
using System.Linq;
using System.Linq.Expressions;

namespace NMetaschema.Model.Common
{
    public interface IMetaschema
    {
        static Expression<Func<IDefinition, bool>> keepAllRootAssemblyDefinitions
            = ModelType.ASSEMBLY.Equals(definition.getModelType()) && (definition as IAssemblyDefinition).isRoot();

        Uri getLocation();
        void getName();
        String getVersion();
        void getRemarks();
        String getShortName();
        Uri getXmlNamespace();
        Uri getJsonBaseUri();
        IList<IMetaschema> getImportedMetaschemas();
        IMetaschema? getImportedMetaschemaByShortName(string shortName);
        IList<IAssemblyDefinition> getAssemblyDefinitions();
        IAssemblyDefinition? getAssemblyDefinitionByName(string name);
        IList<IFieldDefinition> getFieldDefinitions();
        IFieldDefinition? getFieldDefinitionByName(string name);

        IList<INamedModelDefinition> getAssemblyAndFieldDefinitions() {
            return IList<INamedModelDefinition>.Concat(getAssemblyDefinitions(), getFieldDefinitions());
        }

        IList<IFlagDefinition> getFlagDefinitions();
        IFlagDefinition? getFlagDefinitionByName(string name);

        IAssemblyDefinition? getScopedAssemblyDefinitionByName(string name) {
            // first try local/global top-level definitions from current metaschema
            IAssemblyDefinition retval = getAssemblyDefinitionByName(name);
            if (retval == null) {
                // try global definitions from imported metaschema
                retval = getExportedAssemblyDefinitionByName(name);
            }
            return retval;
        }

        IFieldDefinition? getScopedFieldDefinitionByName(string name) {
            // first try local/global top-level definitions from current metaschema
            IFieldDefinition retval = getFieldDefinitionByName(name);
            if (retval == null) {
                // try global definitions from imported metaschema
                retval = getExportedFieldDefinitionByName(name);
            }
            return retval;
        }

        IFlagDefinition? getScopedFlagDefinitionByName(string name) {
            // first try local/global top-level definitions from current metaschema
            IFlagDefinition retval = getFlagDefinitionByName(name);
            if (retval == null) {
                // try global definitions from imported metaschema
                retval = getExportedFlagDefinitionByName(name);
            }
            return retval;
        }

        IList<IAssemblyDefinition> getRootAssemblyDefinitions() {
            return getExportedAssemblyDefinitions().TakeWhile(keepAllRootAssemblyDefinitions);
        }

        ICollection<IFlagDefinition> getExportedFlagDefinitions();
        IFlagDefinition? getExportedFlagDefinitionByName(string name);
        ICollection<IFieldDefinition> getExportedFieldDefinitions();
        IFieldDefinition? getExportedFieldDefinitionByName(string name);
        ICollection<IAssemblyDefinition> getExportedAssemblyDefinitions();
        IAssemblyDefinition? getExportedAssemblyDefinitionByName(string name);
        // TODO: Metapath is very far down the roadmap.
        // void getInfoElementsByMetapath();
    }
}