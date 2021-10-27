using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Irony.Ast;
using Irony.Parsing;

namespace Java.Interop.Tools.JavaSource {

	public partial class SourceJavadocToXmldocGrammar {

		// https://docs.oracle.com/javase/7/docs/technotes/tools/windows/javadoc.html#javadoctags
		public class InlineTagsBnfTerms {

			public InlineTagsBnfTerms ()
			{
			}

			internal void CreateRules (SourceJavadocToXmldocGrammar grammar)
			{
				AllInlineTerms.Rule = CodeDeclaration
					| DocRootDeclaration
					| InheritDocDeclaration
					| LinkDeclaration
					| LinkplainDeclaration
					| LiteralDeclaration
					| ValueDeclaration
					;

				CodeDeclaration.Rule = grammar.ToTerm ("{@code") + InlineValue + "}";
				CodeDeclaration.AstConfig.NodeCreator = (context, parseNode) => {
					parseNode.AstNode = new XElement ("c", parseNode.ChildNodes [1].AstNode.ToString ().Trim ());
				};

				DocRootDeclaration.Rule = grammar.ToTerm ("{@docRoot}");
				DocRootDeclaration.AstConfig.NodeCreator = (context, parseNode) => {
					parseNode.AstNode = new XText ("[TODO: @docRoot]");
				};

				InheritDocDeclaration.Rule = grammar.ToTerm ("{@inheritDoc}");
				InheritDocDeclaration.AstConfig.NodeCreator = (context, parseNode) => {
					parseNode.AstNode = new XText ("[TODO: @inheritDoc]");
				};

				LinkDeclaration.Rule = grammar.ToTerm ("{@link") + InlineValue + "}";
				LinkDeclaration.AstConfig.NodeCreator = (context, parseNode) => {
					// TODO: *everything*; {@link target label}, but target can contain spaces!
					// Also need to convert to appropriate CREF value, use code text for now.
					var target = parseNode.ChildNodes [1].AstNode;
					parseNode.AstNode = new XElement ("c", target);
				};

				LinkplainDeclaration.Rule = grammar.ToTerm ("{@linkplain") + InlineValue + "}";
				LinkplainDeclaration.AstConfig.NodeCreator = (context, parseNode) => {
					// TODO: *everything*; {@link target label}, but target can contain spaces!
					// Also need to convert to appropriate CREF value, use text for now.
					var target = parseNode.ChildNodes [1].AstNode.ToString ();
					parseNode.AstNode = new XText (target);
				};

				LiteralDeclaration.Rule = grammar.ToTerm ("{@literal") + InlineValue + "}";
				LiteralDeclaration.AstConfig.NodeCreator = (context, parseNode) => {
					var content = parseNode.ChildNodes [1].AstNode.ToString ();
					parseNode.AstNode = new XText (content);
				};

				ValueDeclaration.Rule = grammar.ToTerm ("{@value}")
					| grammar.ToTerm ("{@value") + InlineValue + "}";
				ValueDeclaration.AstConfig.NodeCreator = (context, parseNode) => {
					if (parseNode.ChildNodes.Count > 1) {
						var field = parseNode.ChildNodes [1].AstNode.ToString ();
						parseNode.AstNode = new XText ($"[TODO: @value for `{field}`]");
					}
					else {
						parseNode.AstNode = new XText ("[TODO: @value]");
					}
				};
			}

			public  readonly    NonTerminal AllInlineTerms              = new NonTerminal (nameof (AllInlineTerms), ConcatChildNodes);

			public  readonly    Terminal    InlineValue                 = new RegexBasedTerminal (nameof (InlineValue), "[^}]*") {
				AstConfig = new AstNodeConfig {
					NodeCreator = (context, parseNode) => parseNode.AstNode = parseNode.Token.Value,
				},
			};

			// https://docs.oracle.com/javase/7/docs/technotes/tools/windows/javadoc.html#code
			public  readonly    NonTerminal CodeDeclaration             = new NonTerminal (nameof (CodeDeclaration));

			// https://docs.oracle.com/javase/7/docs/technotes/tools/windows/javadoc.html#docRoot
			public  readonly    NonTerminal DocRootDeclaration          = new NonTerminal (nameof (DocRootDeclaration));

			// https://docs.oracle.com/javase/7/docs/technotes/tools/windows/javadoc.html#inheritDoc
			public  readonly    NonTerminal InheritDocDeclaration       = new NonTerminal (nameof (InheritDocDeclaration));

			// https://docs.oracle.com/javase/7/docs/technotes/tools/windows/javadoc.html#link
			public  readonly    NonTerminal LinkDeclaration             = new NonTerminal (nameof (LinkDeclaration));

			// https://docs.oracle.com/javase/7/docs/technotes/tools/windows/javadoc.html#linkplain
			public  readonly    NonTerminal LinkplainDeclaration        = new NonTerminal (nameof (LinkplainDeclaration));

			// https://docs.oracle.com/javase/7/docs/technotes/tools/windows/javadoc.html#literal
			public  readonly    NonTerminal LiteralDeclaration         = new NonTerminal (nameof (LinkplainDeclaration));

			// https://docs.oracle.com/javase/7/docs/technotes/tools/windows/javadoc.html#value
			public  readonly    NonTerminal ValueDeclaration           = new NonTerminal (nameof (ValueDeclaration));
		}
	}
}
