package com.jspecifytest;

import org.jspecify.annotations.Nullable;
import org.jspecify.annotations.NullUnmarked;

/**
 * Lives inside a `@NullMarked` package. Without any annotations,
 * all reference-typed return values, parameters, and fields should
 * be considered non-null.
 */
public class JSpecifyPackageMarked {

	// Reference return / param / field with no annotations -> non-null.
	public String defaultReturn (String value) {
		return value;
	}

	public String defaultField;

	// Primitive return / field — never gets a `not-null` attribute.
	public int primitiveReturn () {
		return 0;
	}

	public int primitiveField;

	// TYPE_USE `@Nullable` overrides scope default.
	public @Nullable String nullableReturn (@Nullable String value) {
		return value;
	}

	public @Nullable String nullableField;

	// `@NullUnmarked` at the method level reverts the scope.
	@NullUnmarked
	public String unmarkedReturn (String value) {
		return value;
	}
}
