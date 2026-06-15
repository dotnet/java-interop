using System;

namespace MonoDroid.Generation {

	internal interface IGeneratable : ISymbol {
		bool IsGeneratable { get; }
		void Generate (CodeGenerationOptions opt, GenerationInfo gen_info);
	}
}
