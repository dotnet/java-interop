package com.microsoft.android;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Collection;
import java.util.Comparator;
import java.util.Enumeration;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;

import com.github.javaparser.*;
import com.github.javaparser.JavaParser;
import com.github.javaparser.ParserConfiguration;
import com.github.javaparser.StaticJavaParser;
import com.github.javaparser.ast.*;
import com.github.javaparser.ast.type.*;
import com.github.javaparser.ast.body.*;
import com.github.javaparser.ast.body.BodyDeclaration;
import com.github.javaparser.ast.body.TypeDeclaration;
import com.github.javaparser.ast.expr.Expression;
import com.github.javaparser.ast.body.Parameter;
import com.github.javaparser.ast.nodeTypes.*;
import com.github.javaparser.ast.nodeTypes.NodeWithJavadoc;
import com.github.javaparser.ast.nodeTypes.NodeWithParameters;
import com.github.javaparser.ast.nodeTypes.NodeWithSimpleName;
import com.github.javaparser.resolution.SymbolResolver;
import com.github.javaparser.resolution.types.ResolvedType;
import com.github.javaparser.symbolsolver.*;
import com.github.javaparser.symbolsolver.model.resolution.TypeSolver;
import com.github.javaparser.symbolsolver.resolution.typesolvers.*;


public class JavaSourceUtilsOptions implements AutoCloseable {
	public static final String HELP_STRING = "[-v] [<-a|--aar> AAR]* [<-j|--jar> JAR]* [<-s|--source> DIRS]*\n" +
		"\t[--bootclasspath CLASSPATH]\n" +
		"\t[<-P|--output-params> OUT.params.txt] [<-D|--output-javadoc> OUT.xml]\n" +
		"\t[@RESPONSE-FILE]* FILES";

	public  static  boolean             verboseOutput;

	public	final   List<File>          aarFiles      = new ArrayList<File>();
	public  final   List<File>          jarFiles      = new ArrayList<File>();

	public  final   Collection<File>    inputFiles    = new ArrayList<File>();

	public  boolean haveBootClassPath;
	public  String  outputParamsTxt;
	public  String  outputJavadocXml;

	private final   Collection<File>    sourceDirectoryFiles  = new ArrayList<File>();
	private         File                extractedTempDir;


	public void close() {
		if (extractedTempDir != null) {
			try {
				Files.walk(extractedTempDir.toPath())
					.sorted(Comparator.reverseOrder())
					.map(Path::toFile)
					.forEach(File::delete);
				extractedTempDir.delete();
			}
			catch (Throwable t) {
				System.err.println(App.APP_NAME + ": error deleting temp directory `" + extractedTempDir.getAbsolutePath() + "`: " + t.getMessage());
				if (verboseOutput) {
					t.printStackTrace(System.err);
				}
			}
		}
		extractedTempDir    = null;
	}

	public ParserConfiguration createConfiguration() throws IOException {
		final   ParserConfiguration config      = new ParserConfiguration()
			// Associate Javadoc comments with AST members
			.setAttributeComments(true)

			// If there are blank lines between Javadoc blocks & declarations,
			// *ignore* those blank lines and associate the Javadoc w/ the decls
			.setDoNotAssignCommentsPrecedingEmptyLines(false)

			// Associate Javadoc comments w/ the declaration, *not* with
			// any annotations on the declaration
			.setIgnoreAnnotationsWhenAttributingComments(true)
			;
		final   TypeSolver          typeSolver  = createTypeSolver(config);
		config.setSymbolResolver(new JavaSymbolSolver(typeSolver));
		return config;
	}

	private final TypeSolver createTypeSolver(ParserConfiguration config) throws IOException {
		final   CombinedTypeSolver  typeSolver  = new CombinedTypeSolver();
		for (File file : aarFiles) {
			typeSolver.add(new AarTypeSolver(file));
		}
		for (File file : jarFiles) {
			typeSolver.add(new JarTypeSolver(file));
		}
		if (!haveBootClassPath) {
			typeSolver.add(new ReflectionTypeSolver());
		}
		for (File srcDir : sourceDirectoryFiles) {
			typeSolver.add(new JavaParserTypeSolver(srcDir, config));
		}
		return typeSolver;
	}

	private final JavaSourceUtilsOptions parse(Iterator<String> args) throws IOException {
		if (args == null || !args.hasNext())
			return this;

		while (args.hasNext()) {
			String arg = args.next();
			switch (arg) {
				case "-bootclasspath": {
					final   String          bootClassPath   = getNextOptionValue(args, arg);
					final   ArrayList<File> files           = new ArrayList<File>();
					for (final String cp : bootClassPath.split(File.pathSeparator)) {
						final   File    file    = new File(cp);
						if (!file.exists()) {
							System.err.println(App.APP_NAME + ": warning: invalid file path for option `-bootclasspath`: " + cp);
							continue;
						}
						files.add(file);
					}
					for (int j = files.size(); j > 0; --j) {
						jarFiles.add(0, files.get(j-1));
					}
					haveBootClassPath   = true;
					break;
				}
				case "-a":
				case "--aar": {
					final   File    file    = getNextOptionFile(args, arg);
					if (file == null) {
						break;
					}
					aarFiles.add(file);
					break;
				}
				case "-j":
				case "--jar": {
					final   File    file    = getNextOptionFile(args, arg);
					if (file == null) {
						break;
					}
					jarFiles.add(file);
					break;
				}
				case "-s":
				case "--source": {
					final   File    dir     = getNextOptionFile(args, arg);
					if (dir == null) {
						break;
					}
					sourceDirectoryFiles.add(dir);
					break;
				}
				case "-D":
				case "--output-javadoc": {
					outputJavadocXml    = getNextOptionValue(args, arg);
					break;
				}
				case "-P":
				case "--output-params": {
					outputParamsTxt     = getNextOptionValue(args, arg);
					break;
				}
				case "-v": {
					verboseOutput   = true;
					break;
				}
				case "-h":
				case "--help": {
					return null;
				}
				default: {
					if (arg.startsWith("@")) {
						// response file?
						final   String  responseFileName = arg.substring(1);
						final   File    responseFile     = new File(responseFileName);
						if (responseFile.exists()) {
							final   Iterator<String>        lines   =
								Files.readAllLines(responseFile.toPath())
								.stream()
								.filter(line -> line.length() > 0 && !line.startsWith("#"))
								.iterator();

							final   JavaSourceUtilsOptions  r       = parse(lines);
							if (r == null)
								return null;
							break;
						}
					}
					final   File    file        = new File(arg);
					if (!file.exists()) {
						System.err.println(App.APP_NAME + ": warning: invalid file path for option `FILES`: " + arg);
						break;
					}

					if (file.isDirectory()) {
						sourceDirectoryFiles.add(file);
						Files.walk(file.toPath())
							.filter(f -> Files.isRegularFile(f) && f.getFileName().toString().endsWith(".java"))
							.map(Path::toFile)
							.forEach(f -> inputFiles.add(f));
						break;
					}
					if (file.getName().endsWith(".java")) {
						inputFiles.add(file);
						break;
					}
					if (!file.getName().endsWith(".jar") && !file.getName().endsWith(".zip")) {
						System.err.println(App.APP_NAME + ": warning: ignoring input file `" + file.getAbsolutePath() +"`.");
						break;
					}
					if (extractedTempDir == null) {
						extractedTempDir    = Files.createTempDirectory("ji-jst").toFile();
					}
					File toDir  = new File(extractedTempDir, file.getName());
					sourceDirectoryFiles.add(toDir);
					extractTo(file, toDir, inputFiles);
					break;
				}
			}
		}
		return this;
	}

	public static JavaSourceUtilsOptions parse(final String[] args) throws IOException {
		final   JavaSourceUtilsOptions  options = new JavaSourceUtilsOptions();
		final   Iterator<String>        a       = Arrays.stream(args).iterator();

		return options.parse(a);
	}

	private static void extractTo(final File zipFilePath, final File toDir, final Collection<File> inputFiles) throws IOException {
		try (final ZipFile zipFile  = new ZipFile(zipFilePath)) {
			Enumeration<? extends ZipEntry> e   = zipFile.entries();
			while (e.hasMoreElements()) {
				final   ZipEntry    entry       = e.nextElement();
				if (entry.isDirectory())
					continue;
				if (!entry.getName().endsWith(".java"))
					continue;
				final   File        target      = new File(toDir, entry.getName());
				if (verboseOutput) {
					System.out.println ("# creating file: " + target.getAbsolutePath());
				}
				target.getParentFile().mkdirs();
				final   InputStream zipContents = zipFile.getInputStream(entry);
				Files.copy(zipContents, target.toPath());
				zipContents.close();
				inputFiles.add(target);
			}
		}
	}

	static String getNextOptionValue(final Iterator<String> args, final String option) {
		if (!args.hasNext())
			throw new IllegalArgumentException(
				"Expected required value for option `" + option + "`.");
		return args.next();
	}

	static File getNextOptionFile(final Iterator<String> args, final String option) {
		if (!args.hasNext())
			throw new IllegalArgumentException(
					"Expected required value for option `" + option + "`.");
		final   String  fileName    = args.next();
		final   File    file        = new File(fileName);
		if (!file.exists()) {
			System.err.println(App.APP_NAME + ": warning: invalid file path for option `" + option + "`: " + fileName);
			return null;
		}
		return file;
	}
}
