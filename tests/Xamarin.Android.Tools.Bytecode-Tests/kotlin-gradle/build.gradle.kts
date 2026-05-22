plugins {
    kotlin("jvm") version "2.0.21"
}

repositories {
    mavenCentral()
}

kotlin {
    jvmToolchain(17)
}

// Emit compiled classes into a stable, predictable location so the
// .NET test harness can load them via ClassFileFixture without needing
// to know the Gradle build directory layout.
tasks.named<org.jetbrains.kotlin.gradle.tasks.KotlinCompile>("compileKotlin") {
    destinationDirectory.set(file("$rootDir/classes"))
}
