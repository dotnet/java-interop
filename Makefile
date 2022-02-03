OS           ?= $(shell uname)

V             ?= 0
CONFIGURATION = Debug

ifeq ($(OS),Darwin)
NATIVE_EXT = .dylib
DLLMAP_OS_NAME = osx
endif
ifeq ($(OS),Linux)
NATIVE_EXT = .so
DLLMAP_OS_NAME = linux
endif

TESTS = \
	bin/Test$(CONFIGURATION)/Java.Interop-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Dynamic-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Export-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Tools.JavaCallableWrappers-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Tools.JavaSource-Tests.dll \
	bin/Test$(CONFIGURATION)/logcat-parse-Tests.dll \
	bin/Test$(CONFIGURATION)/generator-Tests.dll \
	bin/Test$(CONFIGURATION)/Xamarin.Android.Tools.ApiXmlAdjuster-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Tools.JavaTypeSystem-Tests.dll \
	bin/Test$(CONFIGURATION)/Xamarin.Android.Tools.Bytecode-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Tools.Generator-Tests.dll \
	bin/Test$(CONFIGURATION)/Xamarin.SourceWriter-Tests.dll

PTESTS = \
	bin/Test$(CONFIGURATION)/Java.Interop-PerformanceTests.dll

ATESTS = \
	bin/Test$(CONFIGURATION)/Android.Interop-Tests.dll

BUILD_PROPS = bin/Build$(CONFIGURATION)/MonoInfo.props

all:
	msbuild Java.Interop.sln /p:Configuration=$(CONFIGURATION)

run-all-tests:
	r=0; \
	$(MAKE) run-tests                 || r=1 ; \
	$(MAKE) run-test-jnimarshal       || r=1 ; \
	$(MAKE) run-ptests                || r=1 ; \
	$(MAKE) run-java-source-utils-tests     || r=1 ; \
	exit $$r;

include build-tools/scripts/msbuild.mk

prepare:: $(BUILD_PROPS)
	dotnet build Java.Interop.sln -c $(CONFIGURATION) -target:Prepare -p:MaxJdkVersion=$(JI_MAX_JDK)

prepare-core: bin/Build$(CONFIGURATION)/MonoInfo.props

clean:
	-$(MSBUILD) $(MSBUILD_FLAGS) /t:Clean
	-rm -Rf bin/$(CONFIGURATION) bin/Build$(CONFIGURATION) bin/Test$(CONFIGURATION)

include build-tools/scripts/mono.mk
-include bin/Build$(CONFIGURATION)/JdkInfo.mk

JAVA_INTEROP_LIB    = libjava-interop$(NATIVE_EXT)

# Usage: $(call TestAssemblyTemplate,assembly-basename)
define TestAssemblyTemplate
bin/Test$$(CONFIGURATION)/$(1)-Tests.dll: $(wildcard src/$(1)/*/*.cs src/$(1)/Test*/*/*.cs)
	$$(MSBUILD) $$(MSBUILD_FLAGS)
	touch $$@
endef # TestAssemblyTemplate

$(eval $(call TestAssemblyTemplate,Java.Interop))
$(eval $(call TestAssemblyTemplate,Java.Interop.Dynamic))
$(eval $(call TestAssemblyTemplate,Java.Interop.Export))
$(eval $(call TestAssemblyTemplate,Java.Interop.Tools.JavaCallableWrappers))

bin/Test$(CONFIGURATION)/Java.Interop-PerformanceTests.dll: $(wildcard tests/Java.Interop-PerformanceTests/*.cs)
	$(MSBUILD) $(MSBUILD_FLAGS)
	touch $@

bin/Test$(CONFIGURATION)/Android.Interop-Tests.dll: $(wildcard src/Android.Interop/*/*.cs src/Android.Interop/Tests/*/*.cs)
	$(MSBUILD) $(MSBUILD_FLAGS)
	touch $@

bin/$(CONFIGURATION)/Java.Interop.dll: $(wildcard src/Java.Interop/*/*.cs) src/Java.Interop/Java.Interop.csproj
	$(MSBUILD) $(if $(V),/v:diag,) /p:Configuration=$(CONFIGURATION) $(if $(SNK),"/p:AssemblyOriginatorKeyFile=$(SNK)",)

CSHARP_REFS = \
	bin/$(CONFIGURATION)/Java.Interop.dll               \
	bin/$(CONFIGURATION)/Java.Interop.Export.dll        \
	bin/$(CONFIGURATION)/Java.Runtime.Environment.dll   \
	bin/Test$(CONFIGURATION)/TestJVM.dll                    \
	$(PTESTS)                                           \
	$(TESTS)

shell:
	MONO_TRACE_LISTENER=Console.Out \
	MONO_OPTIONS=--debug=casts csharp $(patsubst %,-r:%,$(CSHARP_REFS))

# $(call RUN_TEST,filename,log-lref?)
define RUN_TEST
	$(MSBUILD) $(MSBUILD_FLAGS) build-tools/scripts/RunNUnitTests.targets /p:TestAssembly=$(1) || r=1;
endef

run-tests: $(TESTS) bin/Test$(CONFIGURATION)/$(JAVA_INTEROP_LIB)
	r=0; \
	$(foreach t,$(TESTS), $(call RUN_TEST,$(t),1)) \
	exit $$r;

run-ptests: $(PTESTS) bin/Test$(CONFIGURATION)/$(JAVA_INTEROP_LIB)
	r=0; \
	$(foreach t,$(PTESTS), $(call RUN_TEST,$(t))) \
	exit $$r;

run-java-source-utils-tests:
	$(MSBUILD) $(MSBUILD_FLAGS) tools/java-source-utils/java-source-utils.csproj /t:RunTests

bin/Test$(CONFIGURATION)/$(JAVA_INTEROP_LIB): bin/$(CONFIGURATION)/$(JAVA_INTEROP_LIB)
	cp $< $@

define run-jnimarshalmethod-gen
	MONO_TRACE_LISTENER=Console.Out \
	$(RUNTIME) bin/$(CONFIGURATION)/jnimarshalmethod-gen.exe -v --jvm "$(JI_JVM_PATH)" -L "$(JI_MONO_LIB_PATH)mono/4.5" -L "$(JI_MONO_LIB_PATH)mono/4.5/Facades" $(2) $(1)
endef

run-test-jnimarshal: bin/Test$(CONFIGURATION)/Java.Interop.Export-Tests.dll bin/Test$(CONFIGURATION)/$(JAVA_INTEROP_LIB)
	mkdir -p test-jni-output
	$(call run-jnimarshalmethod-gen,"$<",-f -o test-jni-output --keeptemp)
	(test -f test-jni-output/$(notdir $<) && test -f test-jni-output/Java.Interop.Export-Tests-JniMarshalMethods.dll) || { echo "jnimarshalmethod-gen did not create the expected assemblies in the test-jni-output directory"; exit 1; }
	$(call run-jnimarshalmethod-gen,"$<")
	$(call RUN_TEST,$<)

bin/Test$(CONFIGURATION)/generator.exe: bin/$(CONFIGURATION)/generator.exe
	cp $<* `dirname "$@"`

update-test-generator-nunit:
	-$(MAKE) run-tests TESTS=bin/Test$(CONFIGURATION)/generator-Tests.dll
	for f in `find tests/generator-Tests/expected -name \*.cs` ; do \
		source=`echo $$f | sed 's#^tests/generator-Tests/expected#bin/Test$(CONFIGURATION)/out#'` ; \
		if [ -f "$$source" ]; then \
			cp -f "$$source" "$$f" ; \
		fi; \
	done
	for source in `find bin/Test$(CONFIGURATION)/out.ji -type f` ; do \
		f=`echo $$source | sed 's#^bin/Test$(CONFIGURATION)/out.ji#tests/generator-Tests/expected.ji#'` ; \
		mkdir -p `dirname $$f`; \
		cp -f "$$source" "$$f" ; \
	done
