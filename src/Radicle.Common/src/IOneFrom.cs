namespace Radicle.Common;

using System;
using System.Diagnostics.CodeAnalysis;
using Radicle.Common.Check;
using Radicle.Common.MetaData;

/// <summary>
/// Base interface of all sum types. See <see cref="OneFrom"/>.
/// </summary>
[Experimental("Experimental use now only.")]
public interface IOneFrom
{
    /// <summary>
    /// Gets value of the sum type.
    /// </summary>
    object SumTypeValue { get; }

    /// <summary>
    /// Gets index of the sum type value.
    /// </summary>
    byte SumTypeValueIndex { get; }
}

/// <summary>
/// Sum type of single value. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0> : IOneFrom
    where T0 : notnull
{
    /// <summary>
    /// Pick the single value.
    /// </summary>
    /// <returns>Instance of <typeparamref name="T0"/>.</returns>
    T0 Pick()
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 1);

        return (T0)this.SumTypeValue;
    }

    /// <summary>
    /// Perform action on value.
    /// </summary>
    /// <param name="actionT0">Action.</param>
    void Switch(Action<T0> actionT0)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 1);

        Ensure.Param(actionT0).Value((T0)this.SumTypeValue);
    }

    /// <summary>
    /// Perform mapping of value.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="funcT0">Mapping function.</param>
    /// <returns>Mapped instance.</returns>
    TResult Match<TResult>(Func<T0, TResult> funcT0)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 1);

        return Ensure.Param(funcT0).Value((T0)this.SumTypeValue);
    }
}

/// <summary>
/// Sum type of two values. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0, T1> : IOneFrom
    where T0 : notnull
    where T1 : notnull
{
    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPick(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1>? remainder)
    {
        return this.TryPickT0(out value, out remainder);
    }

    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT0(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 2);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 0)
        {
            value = (T0)this.SumTypeValue;
            return true;
        }

        remainder = OneFrom<T1>.FromT0((T1)this.SumTypeValue);
        return false;
    }

    /// <summary>
    /// Pick the second value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT1(
            [NotNullWhen(returnValue: true)] out T1? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 2);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 1)
        {
            value = (T1)this.SumTypeValue;
            return true;
        }

        remainder = OneFrom<T0>.FromT0((T0)this.SumTypeValue);
        return false;
    }

    /// <summary>
    /// Perform action according to set value.
    /// </summary>
    /// <param name="actionT0">Action to perform if <typeparamref name="T0"/> is set.</param>
    /// <param name="actionT1">Action to perform if <typeparamref name="T1"/> is set.</param>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    void Switch(
            Action<T0> actionT0,
            Action<T1> actionT1)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 2);

        Ensure.Param(actionT0).Done();
        Ensure.Param(actionT1).Done();

        switch (this.SumTypeValueIndex)
        {
            case 0: actionT0((T0)this.SumTypeValue); break;
            case 1: actionT1((T1)this.SumTypeValue); break;
            default:
                throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values");
        }
    }

    /// <summary>
    /// Perform mapping according to set value.
    /// </summary>
    /// <typeparam name="TResult">Mapping type.</typeparam>
    /// <param name="funcT0">Mapping performed if <typeparamref name="T0"/> is set.</param>
    /// <param name="funcT1">Mapping performed if <typeparamref name="T1"/> is set.</param>
    /// <returns>Mapped instance.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    TResult Match<TResult>(
            Func<T0, TResult> funcT0,
            Func<T1, TResult> funcT1)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 2);

        Ensure.Param(funcT0).Done();
        Ensure.Param(funcT1).Done();

        return this.SumTypeValueIndex switch
        {
            0 => funcT0((T0)this.SumTypeValue),
            1 => funcT1((T1)this.SumTypeValue),
            _ => throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values"),
        };
    }
}

/// <summary>
/// Sum type of three values. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0, T1, T2> : IOneFrom
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
{
    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPick(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2>? remainder)
    {
        return this.TryPickT0(out value, out remainder);
    }

    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT0(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 3);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 0)
        {
            value = (T0)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T1, T2>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 0),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the second value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT1(
            [NotNullWhen(returnValue: true)] out T1? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T2>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 3);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 1)
        {
            value = (T1)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T2>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 1),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the third value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT2(
            [NotNullWhen(returnValue: true)] out T2? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 3);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 2)
        {
            value = (T2)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 2),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Perform action according to set value.
    /// </summary>
    /// <param name="actionT0">Action to perform if <typeparamref name="T0"/> is set.</param>
    /// <param name="actionT1">Action to perform if <typeparamref name="T1"/> is set.</param>
    /// <param name="actionT2">Action to perform if <typeparamref name="T2"/> is set.</param>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    void Switch(
            Action<T0> actionT0,
            Action<T1> actionT1,
            Action<T2> actionT2)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 3);

        Ensure.Param(actionT0).Done();
        Ensure.Param(actionT1).Done();
        Ensure.Param(actionT2).Done();

        switch (this.SumTypeValueIndex)
        {
            case 0: actionT0((T0)this.SumTypeValue); break;
            case 1: actionT1((T1)this.SumTypeValue); break;
            case 2: actionT2((T2)this.SumTypeValue); break;
            default:
                throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values");
        }
    }

    /// <summary>
    /// Perform mapping according to set value.
    /// </summary>
    /// <typeparam name="TResult">Mapping type.</typeparam>
    /// <param name="funcT0">Mapping performed if <typeparamref name="T0"/> is set.</param>
    /// <param name="funcT1">Mapping performed if <typeparamref name="T1"/> is set.</param>
    /// <param name="funcT2">Mapping performed if <typeparamref name="T2"/> is set.</param>
    /// <returns>Mapped instance.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    TResult Match<TResult>(
            Func<T0, TResult> funcT0,
            Func<T1, TResult> funcT1,
            Func<T2, TResult> funcT2)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 3);

        Ensure.Param(funcT0).Done();
        Ensure.Param(funcT1).Done();
        Ensure.Param(funcT2).Done();

        return this.SumTypeValueIndex switch
        {
            0 => funcT0((T0)this.SumTypeValue),
            1 => funcT1((T1)this.SumTypeValue),
            2 => funcT2((T2)this.SumTypeValue),
            _ => throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values"),
        };
    }
}

/// <summary>
/// Sum type of four values. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0, T1, T2, T3> : IOneFrom
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
{
    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPick(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3>? remainder)
    {
        return this.TryPickT0(out value, out remainder);
    }

    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT0(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 4);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 0)
        {
            value = (T0)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T1, T2, T3>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 0),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the second value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT1(
            [NotNullWhen(returnValue: true)] out T1? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T2, T3>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 4);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 1)
        {
            value = (T1)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T2, T3>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 1),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the third value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT2(
            [NotNullWhen(returnValue: true)] out T2? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T3>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 4);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 2)
        {
            value = (T2)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T3>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 2),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fourth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT3(
            [NotNullWhen(returnValue: true)] out T3? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 4);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 3)
        {
            value = (T3)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 3),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Perform action according to set value.
    /// </summary>
    /// <param name="actionT0">Action to perform if <typeparamref name="T0"/> is set.</param>
    /// <param name="actionT1">Action to perform if <typeparamref name="T1"/> is set.</param>
    /// <param name="actionT2">Action to perform if <typeparamref name="T2"/> is set.</param>
    /// <param name="actionT3">Action to perform if <typeparamref name="T3"/> is set.</param>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    void Switch(
            Action<T0> actionT0,
            Action<T1> actionT1,
            Action<T2> actionT2,
            Action<T3> actionT3)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 4);

        Ensure.Param(actionT0).Done();
        Ensure.Param(actionT1).Done();
        Ensure.Param(actionT2).Done();
        Ensure.Param(actionT3).Done();

        switch (this.SumTypeValueIndex)
        {
            case 0: actionT0((T0)this.SumTypeValue); break;
            case 1: actionT1((T1)this.SumTypeValue); break;
            case 2: actionT2((T2)this.SumTypeValue); break;
            case 3: actionT3((T3)this.SumTypeValue); break;
            default:
                throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values");
        }
    }

    /// <summary>
    /// Perform mapping according to set value.
    /// </summary>
    /// <typeparam name="TResult">Mapping type.</typeparam>
    /// <param name="funcT0">Mapping performed if <typeparamref name="T0"/> is set.</param>
    /// <param name="funcT1">Mapping performed if <typeparamref name="T1"/> is set.</param>
    /// <param name="funcT2">Mapping performed if <typeparamref name="T2"/> is set.</param>
    /// <param name="funcT3">Mapping performed if <typeparamref name="T3"/> is set.</param>
    /// <returns>Mapped instance.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    TResult Match<TResult>(
            Func<T0, TResult> funcT0,
            Func<T1, TResult> funcT1,
            Func<T2, TResult> funcT2,
            Func<T3, TResult> funcT3)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 4);

        Ensure.Param(funcT0).Done();
        Ensure.Param(funcT1).Done();
        Ensure.Param(funcT2).Done();
        Ensure.Param(funcT3).Done();

        return this.SumTypeValueIndex switch
        {
            0 => funcT0((T0)this.SumTypeValue),
            1 => funcT1((T1)this.SumTypeValue),
            2 => funcT2((T2)this.SumTypeValue),
            3 => funcT3((T3)this.SumTypeValue),
            _ => throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values"),
        };
    }
}

/// <summary>
/// Sum type of five values. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0, T1, T2, T3, T4> : IOneFrom
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
{
    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPick(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4>? remainder)
    {
        return this.TryPickT0(out value, out remainder);
    }

    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT0(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 5);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 0)
        {
            value = (T0)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T1, T2, T3, T4>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 0),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the second value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT1(
            [NotNullWhen(returnValue: true)] out T1? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T2, T3, T4>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 5);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 1)
        {
            value = (T1)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T2, T3, T4>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 1),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the third value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT2(
            [NotNullWhen(returnValue: true)] out T2? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T3, T4>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 5);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 2)
        {
            value = (T2)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T3, T4>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 2),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fourth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT3(
            [NotNullWhen(returnValue: true)] out T3? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T4>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 5);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 3)
        {
            value = (T3)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T4>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 3),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fifth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT4(
            [NotNullWhen(returnValue: true)] out T4? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 5);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 4)
        {
            value = (T4)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 4),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Perform action according to set value.
    /// </summary>
    /// <param name="actionT0">Action to perform if <typeparamref name="T0"/> is set.</param>
    /// <param name="actionT1">Action to perform if <typeparamref name="T1"/> is set.</param>
    /// <param name="actionT2">Action to perform if <typeparamref name="T2"/> is set.</param>
    /// <param name="actionT3">Action to perform if <typeparamref name="T3"/> is set.</param>
    /// <param name="actionT4">Action to perform if <typeparamref name="T4"/> is set.</param>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    void Switch(
            Action<T0> actionT0,
            Action<T1> actionT1,
            Action<T2> actionT2,
            Action<T3> actionT3,
            Action<T4> actionT4)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 5);

        Ensure.Param(actionT0).Done();
        Ensure.Param(actionT1).Done();
        Ensure.Param(actionT2).Done();
        Ensure.Param(actionT3).Done();
        Ensure.Param(actionT4).Done();

        switch (this.SumTypeValueIndex)
        {
            case 0: actionT0((T0)this.SumTypeValue); break;
            case 1: actionT1((T1)this.SumTypeValue); break;
            case 2: actionT2((T2)this.SumTypeValue); break;
            case 3: actionT3((T3)this.SumTypeValue); break;
            case 4: actionT4((T4)this.SumTypeValue); break;
            default:
                throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values");
        }
    }

    /// <summary>
    /// Perform mapping according to set value.
    /// </summary>
    /// <typeparam name="TResult">Mapping type.</typeparam>
    /// <param name="funcT0">Mapping performed if <typeparamref name="T0"/> is set.</param>
    /// <param name="funcT1">Mapping performed if <typeparamref name="T1"/> is set.</param>
    /// <param name="funcT2">Mapping performed if <typeparamref name="T2"/> is set.</param>
    /// <param name="funcT3">Mapping performed if <typeparamref name="T3"/> is set.</param>
    /// <param name="funcT4">Mapping performed if <typeparamref name="T4"/> is set.</param>
    /// <returns>Mapped instance.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    TResult Match<TResult>(
            Func<T0, TResult> funcT0,
            Func<T1, TResult> funcT1,
            Func<T2, TResult> funcT2,
            Func<T3, TResult> funcT3,
            Func<T4, TResult> funcT4)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 5);

        Ensure.Param(funcT0).Done();
        Ensure.Param(funcT1).Done();
        Ensure.Param(funcT2).Done();
        Ensure.Param(funcT3).Done();
        Ensure.Param(funcT4).Done();

        return this.SumTypeValueIndex switch
        {
            0 => funcT0((T0)this.SumTypeValue),
            1 => funcT1((T1)this.SumTypeValue),
            2 => funcT2((T2)this.SumTypeValue),
            3 => funcT3((T3)this.SumTypeValue),
            4 => funcT4((T4)this.SumTypeValue),
            _ => throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values"),
        };
    }
}

/// <summary>
/// Sum type of six values. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0, T1, T2, T3, T4, T5> : IOneFrom
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
{
    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPick(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5>? remainder)
    {
        return this.TryPickT0(out value, out remainder);
    }

    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT0(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 6);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 0)
        {
            value = (T0)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T1, T2, T3, T4, T5>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 0),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the second value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT1(
            [NotNullWhen(returnValue: true)] out T1? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T2, T3, T4, T5>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 6);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 1)
        {
            value = (T1)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T2, T3, T4, T5>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 1),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the third value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT2(
            [NotNullWhen(returnValue: true)] out T2? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T3, T4, T5>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 6);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 2)
        {
            value = (T2)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T3, T4, T5>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 2),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fourth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT3(
            [NotNullWhen(returnValue: true)] out T3? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T4, T5>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 6);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 3)
        {
            value = (T3)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T4, T5>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 3),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fifth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT4(
            [NotNullWhen(returnValue: true)] out T4? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T5>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 6);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 4)
        {
            value = (T4)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T5>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 4),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the sixth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT5(
            [NotNullWhen(returnValue: true)] out T5? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 6);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 5)
        {
            value = (T5)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 5),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Perform action according to set value.
    /// </summary>
    /// <param name="actionT0">Action to perform if <typeparamref name="T0"/> is set.</param>
    /// <param name="actionT1">Action to perform if <typeparamref name="T1"/> is set.</param>
    /// <param name="actionT2">Action to perform if <typeparamref name="T2"/> is set.</param>
    /// <param name="actionT3">Action to perform if <typeparamref name="T3"/> is set.</param>
    /// <param name="actionT4">Action to perform if <typeparamref name="T4"/> is set.</param>
    /// <param name="actionT5">Action to perform if <typeparamref name="T5"/> is set.</param>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    void Switch(
            Action<T0> actionT0,
            Action<T1> actionT1,
            Action<T2> actionT2,
            Action<T3> actionT3,
            Action<T4> actionT4,
            Action<T5> actionT5)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 6);

        Ensure.Param(actionT0).Done();
        Ensure.Param(actionT1).Done();
        Ensure.Param(actionT2).Done();
        Ensure.Param(actionT3).Done();
        Ensure.Param(actionT4).Done();
        Ensure.Param(actionT5).Done();

        switch (this.SumTypeValueIndex)
        {
            case 0: actionT0((T0)this.SumTypeValue); break;
            case 1: actionT1((T1)this.SumTypeValue); break;
            case 2: actionT2((T2)this.SumTypeValue); break;
            case 3: actionT3((T3)this.SumTypeValue); break;
            case 4: actionT4((T4)this.SumTypeValue); break;
            case 5: actionT5((T5)this.SumTypeValue); break;
            default:
                throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values");
        }
    }

    /// <summary>
    /// Perform mapping according to set value.
    /// </summary>
    /// <typeparam name="TResult">Mapping type.</typeparam>
    /// <param name="funcT0">Mapping performed if <typeparamref name="T0"/> is set.</param>
    /// <param name="funcT1">Mapping performed if <typeparamref name="T1"/> is set.</param>
    /// <param name="funcT2">Mapping performed if <typeparamref name="T2"/> is set.</param>
    /// <param name="funcT3">Mapping performed if <typeparamref name="T3"/> is set.</param>
    /// <param name="funcT4">Mapping performed if <typeparamref name="T4"/> is set.</param>
    /// <param name="funcT5">Mapping performed if <typeparamref name="T5"/> is set.</param>
    /// <returns>Mapped instance.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    TResult Match<TResult>(
            Func<T0, TResult> funcT0,
            Func<T1, TResult> funcT1,
            Func<T2, TResult> funcT2,
            Func<T3, TResult> funcT3,
            Func<T4, TResult> funcT4,
            Func<T5, TResult> funcT5)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 6);

        Ensure.Param(funcT0).Done();
        Ensure.Param(funcT1).Done();
        Ensure.Param(funcT2).Done();
        Ensure.Param(funcT3).Done();
        Ensure.Param(funcT4).Done();
        Ensure.Param(funcT5).Done();

        return this.SumTypeValueIndex switch
        {
            0 => funcT0((T0)this.SumTypeValue),
            1 => funcT1((T1)this.SumTypeValue),
            2 => funcT2((T2)this.SumTypeValue),
            3 => funcT3((T3)this.SumTypeValue),
            4 => funcT4((T4)this.SumTypeValue),
            5 => funcT5((T5)this.SumTypeValue),
            _ => throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values"),
        };
    }
}

/// <summary>
/// Sum type of seven values. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
/// <typeparam name="T6">Type of 7th argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0, T1, T2, T3, T4, T5, T6> : IOneFrom
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where T6 : notnull
{
    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPick(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5, T6>? remainder)
    {
        return this.TryPickT0(out value, out remainder);
    }

    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT0(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5, T6>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 7);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 0)
        {
            value = (T0)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T1, T2, T3, T4, T5, T6>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 0),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the second value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT1(
            [NotNullWhen(returnValue: true)] out T1? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T2, T3, T4, T5, T6>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 7);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 1)
        {
            value = (T1)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T2, T3, T4, T5, T6>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 1),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the third value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT2(
            [NotNullWhen(returnValue: true)] out T2? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T3, T4, T5, T6>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 7);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 2)
        {
            value = (T2)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T3, T4, T5, T6>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 2),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fourth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT3(
            [NotNullWhen(returnValue: true)] out T3? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T4, T5, T6>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 7);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 3)
        {
            value = (T3)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T4, T5, T6>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 3),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fifth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT4(
            [NotNullWhen(returnValue: true)] out T4? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T5, T6>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 7);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 4)
        {
            value = (T4)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T5, T6>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 4),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the sixth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT5(
            [NotNullWhen(returnValue: true)] out T5? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T6>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 7);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 5)
        {
            value = (T5)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T6>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 5),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the seventh value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT6(
            [NotNullWhen(returnValue: true)] out T6? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 7);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 6)
        {
            value = (T6)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 6),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Perform action according to set value.
    /// </summary>
    /// <param name="actionT0">Action to perform if <typeparamref name="T0"/> is set.</param>
    /// <param name="actionT1">Action to perform if <typeparamref name="T1"/> is set.</param>
    /// <param name="actionT2">Action to perform if <typeparamref name="T2"/> is set.</param>
    /// <param name="actionT3">Action to perform if <typeparamref name="T3"/> is set.</param>
    /// <param name="actionT4">Action to perform if <typeparamref name="T4"/> is set.</param>
    /// <param name="actionT5">Action to perform if <typeparamref name="T5"/> is set.</param>
    /// <param name="actionT6">Action to perform if <typeparamref name="T6"/> is set.</param>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    void Switch(
            Action<T0> actionT0,
            Action<T1> actionT1,
            Action<T2> actionT2,
            Action<T3> actionT3,
            Action<T4> actionT4,
            Action<T5> actionT5,
            Action<T6> actionT6)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 7);

        Ensure.Param(actionT0).Done();
        Ensure.Param(actionT1).Done();
        Ensure.Param(actionT2).Done();
        Ensure.Param(actionT3).Done();
        Ensure.Param(actionT4).Done();
        Ensure.Param(actionT5).Done();
        Ensure.Param(actionT6).Done();

        switch (this.SumTypeValueIndex)
        {
            case 0: actionT0((T0)this.SumTypeValue); break;
            case 1: actionT1((T1)this.SumTypeValue); break;
            case 2: actionT2((T2)this.SumTypeValue); break;
            case 3: actionT3((T3)this.SumTypeValue); break;
            case 4: actionT4((T4)this.SumTypeValue); break;
            case 5: actionT5((T5)this.SumTypeValue); break;
            case 6: actionT6((T6)this.SumTypeValue); break;
            default:
                throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values");
        }
    }

    /// <summary>
    /// Perform mapping according to set value.
    /// </summary>
    /// <typeparam name="TResult">Mapping type.</typeparam>
    /// <param name="funcT0">Mapping performed if <typeparamref name="T0"/> is set.</param>
    /// <param name="funcT1">Mapping performed if <typeparamref name="T1"/> is set.</param>
    /// <param name="funcT2">Mapping performed if <typeparamref name="T2"/> is set.</param>
    /// <param name="funcT3">Mapping performed if <typeparamref name="T3"/> is set.</param>
    /// <param name="funcT4">Mapping performed if <typeparamref name="T4"/> is set.</param>
    /// <param name="funcT5">Mapping performed if <typeparamref name="T5"/> is set.</param>
    /// <param name="funcT6">Mapping performed if <typeparamref name="T6"/> is set.</param>
    /// <returns>Mapped instance.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    TResult Match<TResult>(
            Func<T0, TResult> funcT0,
            Func<T1, TResult> funcT1,
            Func<T2, TResult> funcT2,
            Func<T3, TResult> funcT3,
            Func<T4, TResult> funcT4,
            Func<T5, TResult> funcT5,
            Func<T6, TResult> funcT6)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 7);

        Ensure.Param(funcT0).Done();
        Ensure.Param(funcT1).Done();
        Ensure.Param(funcT2).Done();
        Ensure.Param(funcT3).Done();
        Ensure.Param(funcT4).Done();
        Ensure.Param(funcT5).Done();
        Ensure.Param(funcT6).Done();

        return this.SumTypeValueIndex switch
        {
            0 => funcT0((T0)this.SumTypeValue),
            1 => funcT1((T1)this.SumTypeValue),
            2 => funcT2((T2)this.SumTypeValue),
            3 => funcT3((T3)this.SumTypeValue),
            4 => funcT4((T4)this.SumTypeValue),
            5 => funcT5((T5)this.SumTypeValue),
            6 => funcT6((T6)this.SumTypeValue),
            _ => throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values"),
        };
    }
}

/// <summary>
/// Sum type of eight values. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
/// <typeparam name="T6">Type of 7th argument.</typeparam>
/// <typeparam name="T7">Type of 8th argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> : IOneFrom
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where T6 : notnull
    where T7 : notnull
{
    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPick(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5, T6, T7>? remainder)
    {
        return this.TryPickT0(out value, out remainder);
    }

    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT0(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5, T6, T7>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 0)
        {
            value = (T0)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T1, T2, T3, T4, T5, T6, T7>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 0),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the second value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT1(
            [NotNullWhen(returnValue: true)] out T1? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T2, T3, T4, T5, T6, T7>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 1)
        {
            value = (T1)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T2, T3, T4, T5, T6, T7>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 1),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the third value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT2(
            [NotNullWhen(returnValue: true)] out T2? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T3, T4, T5, T6, T7>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 2)
        {
            value = (T2)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T3, T4, T5, T6, T7>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 2),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fourth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT3(
            [NotNullWhen(returnValue: true)] out T3? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T4, T5, T6, T7>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 3)
        {
            value = (T3)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T4, T5, T6, T7>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 3),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fifth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT4(
            [NotNullWhen(returnValue: true)] out T4? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T5, T6, T7>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 4)
        {
            value = (T4)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T5, T6, T7>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 4),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the sixth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT5(
            [NotNullWhen(returnValue: true)] out T5? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T6, T7>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 5)
        {
            value = (T5)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T6, T7>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 5),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the seventh value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT6(
            [NotNullWhen(returnValue: true)] out T6? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5, T7>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 6)
        {
            value = (T6)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5, T7>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 6),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the eight value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT7(
            [NotNullWhen(returnValue: true)] out T7? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5, T6>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 7)
        {
            value = (T7)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5, T6>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 7),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Perform action according to set value.
    /// </summary>
    /// <param name="actionT0">Action to perform if <typeparamref name="T0"/> is set.</param>
    /// <param name="actionT1">Action to perform if <typeparamref name="T1"/> is set.</param>
    /// <param name="actionT2">Action to perform if <typeparamref name="T2"/> is set.</param>
    /// <param name="actionT3">Action to perform if <typeparamref name="T3"/> is set.</param>
    /// <param name="actionT4">Action to perform if <typeparamref name="T4"/> is set.</param>
    /// <param name="actionT5">Action to perform if <typeparamref name="T5"/> is set.</param>
    /// <param name="actionT6">Action to perform if <typeparamref name="T6"/> is set.</param>
    /// <param name="actionT7">Action to perform if <typeparamref name="T7"/> is set.</param>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    void Switch(
            Action<T0> actionT0,
            Action<T1> actionT1,
            Action<T2> actionT2,
            Action<T3> actionT3,
            Action<T4> actionT4,
            Action<T5> actionT5,
            Action<T6> actionT6,
            Action<T7> actionT7)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        Ensure.Param(actionT0).Done();
        Ensure.Param(actionT1).Done();
        Ensure.Param(actionT2).Done();
        Ensure.Param(actionT3).Done();
        Ensure.Param(actionT4).Done();
        Ensure.Param(actionT5).Done();
        Ensure.Param(actionT6).Done();
        Ensure.Param(actionT7).Done();

        switch (this.SumTypeValueIndex)
        {
            case 0: actionT0((T0)this.SumTypeValue); break;
            case 1: actionT1((T1)this.SumTypeValue); break;
            case 2: actionT2((T2)this.SumTypeValue); break;
            case 3: actionT3((T3)this.SumTypeValue); break;
            case 4: actionT4((T4)this.SumTypeValue); break;
            case 5: actionT5((T5)this.SumTypeValue); break;
            case 6: actionT6((T6)this.SumTypeValue); break;
            case 7: actionT7((T7)this.SumTypeValue); break;
            default:
                throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values");
        }
    }

    /// <summary>
    /// Perform mapping according to set value.
    /// </summary>
    /// <typeparam name="TResult">Mapping type.</typeparam>
    /// <param name="funcT0">Mapping performed if <typeparamref name="T0"/> is set.</param>
    /// <param name="funcT1">Mapping performed if <typeparamref name="T1"/> is set.</param>
    /// <param name="funcT2">Mapping performed if <typeparamref name="T2"/> is set.</param>
    /// <param name="funcT3">Mapping performed if <typeparamref name="T3"/> is set.</param>
    /// <param name="funcT4">Mapping performed if <typeparamref name="T4"/> is set.</param>
    /// <param name="funcT5">Mapping performed if <typeparamref name="T5"/> is set.</param>
    /// <param name="funcT6">Mapping performed if <typeparamref name="T6"/> is set.</param>
    /// <param name="funcT7">Mapping performed if <typeparamref name="T7"/> is set.</param>
    /// <returns>Mapped instance.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    TResult Match<TResult>(
            Func<T0, TResult> funcT0,
            Func<T1, TResult> funcT1,
            Func<T2, TResult> funcT2,
            Func<T3, TResult> funcT3,
            Func<T4, TResult> funcT4,
            Func<T5, TResult> funcT5,
            Func<T6, TResult> funcT6,
            Func<T7, TResult> funcT7)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 8);

        Ensure.Param(funcT0).Done();
        Ensure.Param(funcT1).Done();
        Ensure.Param(funcT2).Done();
        Ensure.Param(funcT3).Done();
        Ensure.Param(funcT4).Done();
        Ensure.Param(funcT5).Done();
        Ensure.Param(funcT6).Done();
        Ensure.Param(funcT7).Done();

        return this.SumTypeValueIndex switch
        {
            0 => funcT0((T0)this.SumTypeValue),
            1 => funcT1((T1)this.SumTypeValue),
            2 => funcT2((T2)this.SumTypeValue),
            3 => funcT3((T3)this.SumTypeValue),
            4 => funcT4((T4)this.SumTypeValue),
            5 => funcT5((T5)this.SumTypeValue),
            6 => funcT6((T6)this.SumTypeValue),
            7 => funcT7((T7)this.SumTypeValue),
            _ => throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values"),
        };
    }
}

/// <summary>
/// Sum type of nine values. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
/// <typeparam name="T6">Type of 7th argument.</typeparam>
/// <typeparam name="T7">Type of 8th argument.</typeparam>
/// <typeparam name="T8">Type of 9th argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> : IOneFrom
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where T6 : notnull
    where T7 : notnull
    where T8 : notnull
{
    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPick(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5, T6, T7, T8>? remainder)
    {
        return this.TryPickT0(out value, out remainder);
    }

    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT0(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5, T6, T7, T8>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 0)
        {
            value = (T0)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T1, T2, T3, T4, T5, T6, T7, T8>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 0),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the second value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT1(
            [NotNullWhen(returnValue: true)] out T1? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T2, T3, T4, T5, T6, T7, T8>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 1)
        {
            value = (T1)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T2, T3, T4, T5, T6, T7, T8>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 1),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the third value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT2(
            [NotNullWhen(returnValue: true)] out T2? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T3, T4, T5, T6, T7, T8>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 2)
        {
            value = (T2)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T3, T4, T5, T6, T7, T8>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 2),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fourth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT3(
            [NotNullWhen(returnValue: true)] out T3? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T4, T5, T6, T7, T8>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 3)
        {
            value = (T3)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T4, T5, T6, T7, T8>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 3),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fifth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT4(
            [NotNullWhen(returnValue: true)] out T4? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T5, T6, T7, T8>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 4)
        {
            value = (T4)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T5, T6, T7, T8>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 4),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the sixth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT5(
            [NotNullWhen(returnValue: true)] out T5? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T6, T7, T8>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 5)
        {
            value = (T5)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T6, T7, T8>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 5),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the seventh value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT6(
            [NotNullWhen(returnValue: true)] out T6? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5, T7, T8>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 6)
        {
            value = (T6)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5, T7, T8>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 6),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the eight value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT7(
            [NotNullWhen(returnValue: true)] out T7? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5, T6, T8>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 7)
        {
            value = (T7)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5, T6, T8>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 7),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the ninth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT8(
            [NotNullWhen(returnValue: true)] out T8? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 8)
        {
            value = (T8)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 8),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Perform action according to set value.
    /// </summary>
    /// <param name="actionT0">Action to perform if <typeparamref name="T0"/> is set.</param>
    /// <param name="actionT1">Action to perform if <typeparamref name="T1"/> is set.</param>
    /// <param name="actionT2">Action to perform if <typeparamref name="T2"/> is set.</param>
    /// <param name="actionT3">Action to perform if <typeparamref name="T3"/> is set.</param>
    /// <param name="actionT4">Action to perform if <typeparamref name="T4"/> is set.</param>
    /// <param name="actionT5">Action to perform if <typeparamref name="T5"/> is set.</param>
    /// <param name="actionT6">Action to perform if <typeparamref name="T6"/> is set.</param>
    /// <param name="actionT7">Action to perform if <typeparamref name="T7"/> is set.</param>
    /// <param name="actionT8">Action to perform if <typeparamref name="T8"/> is set.</param>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    void Switch(
            Action<T0> actionT0,
            Action<T1> actionT1,
            Action<T2> actionT2,
            Action<T3> actionT3,
            Action<T4> actionT4,
            Action<T5> actionT5,
            Action<T6> actionT6,
            Action<T7> actionT7,
            Action<T8> actionT8)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        Ensure.Param(actionT0).Done();
        Ensure.Param(actionT1).Done();
        Ensure.Param(actionT2).Done();
        Ensure.Param(actionT3).Done();
        Ensure.Param(actionT4).Done();
        Ensure.Param(actionT5).Done();
        Ensure.Param(actionT6).Done();
        Ensure.Param(actionT7).Done();
        Ensure.Param(actionT8).Done();

        switch (this.SumTypeValueIndex)
        {
            case 0: actionT0((T0)this.SumTypeValue); break;
            case 1: actionT1((T1)this.SumTypeValue); break;
            case 2: actionT2((T2)this.SumTypeValue); break;
            case 3: actionT3((T3)this.SumTypeValue); break;
            case 4: actionT4((T4)this.SumTypeValue); break;
            case 5: actionT5((T5)this.SumTypeValue); break;
            case 6: actionT6((T6)this.SumTypeValue); break;
            case 7: actionT7((T7)this.SumTypeValue); break;
            case 8: actionT8((T8)this.SumTypeValue); break;
            default:
                throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values");
        }
    }

    /// <summary>
    /// Perform mapping according to set value.
    /// </summary>
    /// <typeparam name="TResult">Mapping type.</typeparam>
    /// <param name="funcT0">Mapping performed if <typeparamref name="T0"/> is set.</param>
    /// <param name="funcT1">Mapping performed if <typeparamref name="T1"/> is set.</param>
    /// <param name="funcT2">Mapping performed if <typeparamref name="T2"/> is set.</param>
    /// <param name="funcT3">Mapping performed if <typeparamref name="T3"/> is set.</param>
    /// <param name="funcT4">Mapping performed if <typeparamref name="T4"/> is set.</param>
    /// <param name="funcT5">Mapping performed if <typeparamref name="T5"/> is set.</param>
    /// <param name="funcT6">Mapping performed if <typeparamref name="T6"/> is set.</param>
    /// <param name="funcT7">Mapping performed if <typeparamref name="T7"/> is set.</param>
    /// <param name="funcT8">Mapping performed if <typeparamref name="T8"/> is set.</param>
    /// <returns>Mapped instance.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    TResult Match<TResult>(
            Func<T0, TResult> funcT0,
            Func<T1, TResult> funcT1,
            Func<T2, TResult> funcT2,
            Func<T3, TResult> funcT3,
            Func<T4, TResult> funcT4,
            Func<T5, TResult> funcT5,
            Func<T6, TResult> funcT6,
            Func<T7, TResult> funcT7,
            Func<T8, TResult> funcT8)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 9);

        Ensure.Param(funcT0).Done();
        Ensure.Param(funcT1).Done();
        Ensure.Param(funcT2).Done();
        Ensure.Param(funcT3).Done();
        Ensure.Param(funcT4).Done();
        Ensure.Param(funcT5).Done();
        Ensure.Param(funcT6).Done();
        Ensure.Param(funcT7).Done();
        Ensure.Param(funcT8).Done();

        return this.SumTypeValueIndex switch
        {
            0 => funcT0((T0)this.SumTypeValue),
            1 => funcT1((T1)this.SumTypeValue),
            2 => funcT2((T2)this.SumTypeValue),
            3 => funcT3((T3)this.SumTypeValue),
            4 => funcT4((T4)this.SumTypeValue),
            5 => funcT5((T5)this.SumTypeValue),
            6 => funcT6((T6)this.SumTypeValue),
            7 => funcT7((T7)this.SumTypeValue),
            8 => funcT8((T8)this.SumTypeValue),
            _ => throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values"),
        };
    }
}

/// <summary>
/// Sum type of ten values. See <see cref="OneFrom"/>.
/// </summary>
/// <remarks>
/// <para>
/// See https://en.wikipedia.org/wiki/Algebraic_data_type
/// and https://github.com/mcintyre321/OneOf for naming convention
/// in order to allow simultaneous use with minimal friction.
/// </para>
/// <para>
/// With this interface the types can act like sum types and
/// allow easy discovery of finite derived types and or
/// methods can return ad-hoc created sum types.
/// </para>
/// </remarks>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
/// <typeparam name="T6">Type of 7th argument.</typeparam>
/// <typeparam name="T7">Type of 8th argument.</typeparam>
/// <typeparam name="T8">Type of 9th argument.</typeparam>
/// <typeparam name="T9">Type of 10th argument.</typeparam>
[Experimental("Experimental use now only.")]
public interface IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IOneFrom
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where T6 : notnull
    where T7 : notnull
    where T8 : notnull
    where T9 : notnull
{
    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPick(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5, T6, T7, T8, T9>? remainder)
    {
        return this.TryPickT0(out value, out remainder);
    }

    /// <summary>
    /// Pick the first value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT0(
            [NotNullWhen(returnValue: true)] out T0? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T1, T2, T3, T4, T5, T6, T7, T8, T9>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 0)
        {
            value = (T0)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 0),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the second value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT1(
            [NotNullWhen(returnValue: true)] out T1? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T2, T3, T4, T5, T6, T7, T8, T9>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 1)
        {
            value = (T1)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T2, T3, T4, T5, T6, T7, T8, T9>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 1),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the third value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT2(
            [NotNullWhen(returnValue: true)] out T2? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T3, T4, T5, T6, T7, T8, T9>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 2)
        {
            value = (T2)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T3, T4, T5, T6, T7, T8, T9>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 2),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fourth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT3(
            [NotNullWhen(returnValue: true)] out T3? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T4, T5, T6, T7, T8, T9>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 3)
        {
            value = (T3)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T4, T5, T6, T7, T8, T9>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 3),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the fifth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT4(
            [NotNullWhen(returnValue: true)] out T4? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T5, T6, T7, T8, T9>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 4)
        {
            value = (T4)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T5, T6, T7, T8, T9>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 4),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the sixth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT5(
            [NotNullWhen(returnValue: true)] out T5? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T6, T7, T8, T9>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 5)
        {
            value = (T5)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T6, T7, T8, T9>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 5),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the seventh value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT6(
            [NotNullWhen(returnValue: true)] out T6? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5, T7, T8, T9>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 6)
        {
            value = (T6)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5, T7, T8, T9>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 6),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the eight value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT7(
            [NotNullWhen(returnValue: true)] out T7? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5, T6, T8, T9>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 7)
        {
            value = (T7)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5, T6, T8, T9>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 7),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the ninth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT8(
            [NotNullWhen(returnValue: true)] out T8? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T9>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 8)
        {
            value = (T8)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T9>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 8),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Pick the tenth value.
    /// </summary>
    /// <param name="value">Picked value.</param>
    /// <param name="remainder">Remained after unsuccessful pick.</param>
    /// <returns><see langword="true"/> if value was set; <see langword="false"/> otherwise.</returns>
    bool TryPickT9(
            [NotNullWhen(returnValue: true)] out T9? value,
            [NotNullWhen(returnValue: false)] out IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>? remainder)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        value = default;
        remainder = default;

        if (this.SumTypeValueIndex == 9)
        {
            value = (T9)this.SumTypeValue;
            return true;
        }

        remainder = new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(
                OneFrom.ConvertIndex(this.SumTypeValueIndex, 9),
                this.SumTypeValue);

        return false;
    }

    /// <summary>
    /// Perform action according to set value.
    /// </summary>
    /// <param name="actionT0">Action to perform if <typeparamref name="T0"/> is set.</param>
    /// <param name="actionT1">Action to perform if <typeparamref name="T1"/> is set.</param>
    /// <param name="actionT2">Action to perform if <typeparamref name="T2"/> is set.</param>
    /// <param name="actionT3">Action to perform if <typeparamref name="T3"/> is set.</param>
    /// <param name="actionT4">Action to perform if <typeparamref name="T4"/> is set.</param>
    /// <param name="actionT5">Action to perform if <typeparamref name="T5"/> is set.</param>
    /// <param name="actionT6">Action to perform if <typeparamref name="T6"/> is set.</param>
    /// <param name="actionT7">Action to perform if <typeparamref name="T7"/> is set.</param>
    /// <param name="actionT8">Action to perform if <typeparamref name="T8"/> is set.</param>
    /// <param name="actionT9">Action to perform if <typeparamref name="T9"/> is set.</param>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    void Switch(
            Action<T0> actionT0,
            Action<T1> actionT1,
            Action<T2> actionT2,
            Action<T3> actionT3,
            Action<T4> actionT4,
            Action<T5> actionT5,
            Action<T6> actionT6,
            Action<T7> actionT7,
            Action<T8> actionT8,
            Action<T9> actionT9)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        Ensure.Param(actionT0).Done();
        Ensure.Param(actionT1).Done();
        Ensure.Param(actionT2).Done();
        Ensure.Param(actionT3).Done();
        Ensure.Param(actionT4).Done();
        Ensure.Param(actionT5).Done();
        Ensure.Param(actionT6).Done();
        Ensure.Param(actionT7).Done();
        Ensure.Param(actionT8).Done();
        Ensure.Param(actionT9).Done();

        switch (this.SumTypeValueIndex)
        {
            case 0: actionT0((T0)this.SumTypeValue); break;
            case 1: actionT1((T1)this.SumTypeValue); break;
            case 2: actionT2((T2)this.SumTypeValue); break;
            case 3: actionT3((T3)this.SumTypeValue); break;
            case 4: actionT4((T4)this.SumTypeValue); break;
            case 5: actionT5((T5)this.SumTypeValue); break;
            case 6: actionT6((T6)this.SumTypeValue); break;
            case 7: actionT7((T7)this.SumTypeValue); break;
            case 8: actionT8((T8)this.SumTypeValue); break;
            case 9: actionT9((T9)this.SumTypeValue); break;
            default:
                throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values");
        }
    }

    /// <summary>
    /// Perform mapping according to set value.
    /// </summary>
    /// <typeparam name="TResult">Mapping type.</typeparam>
    /// <param name="funcT0">Mapping performed if <typeparamref name="T0"/> is set.</param>
    /// <param name="funcT1">Mapping performed if <typeparamref name="T1"/> is set.</param>
    /// <param name="funcT2">Mapping performed if <typeparamref name="T2"/> is set.</param>
    /// <param name="funcT3">Mapping performed if <typeparamref name="T3"/> is set.</param>
    /// <param name="funcT4">Mapping performed if <typeparamref name="T4"/> is set.</param>
    /// <param name="funcT5">Mapping performed if <typeparamref name="T5"/> is set.</param>
    /// <param name="funcT6">Mapping performed if <typeparamref name="T6"/> is set.</param>
    /// <param name="funcT7">Mapping performed if <typeparamref name="T7"/> is set.</param>
    /// <param name="funcT8">Mapping performed if <typeparamref name="T8"/> is set.</param>
    /// <param name="funcT9">Mapping performed if <typeparamref name="T9"/> is set.</param>
    /// <returns>Mapped instance.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    TResult Match<TResult>(
            Func<T0, TResult> funcT0,
            Func<T1, TResult> funcT1,
            Func<T2, TResult> funcT2,
            Func<T3, TResult> funcT3,
            Func<T4, TResult> funcT4,
            Func<T5, TResult> funcT5,
            Func<T6, TResult> funcT6,
            Func<T7, TResult> funcT7,
            Func<T8, TResult> funcT8,
            Func<T9, TResult> funcT9)
    {
        OneFrom.EnsureCardinality(this.SumTypeValueIndex, 10);

        Ensure.Param(funcT0).Done();
        Ensure.Param(funcT1).Done();
        Ensure.Param(funcT2).Done();
        Ensure.Param(funcT3).Done();
        Ensure.Param(funcT4).Done();
        Ensure.Param(funcT5).Done();
        Ensure.Param(funcT6).Done();
        Ensure.Param(funcT7).Done();
        Ensure.Param(funcT8).Done();
        Ensure.Param(funcT9).Done();

        return this.SumTypeValueIndex switch
        {
            0 => funcT0((T0)this.SumTypeValue),
            1 => funcT1((T1)this.SumTypeValue),
            2 => funcT2((T2)this.SumTypeValue),
            3 => funcT3((T3)this.SumTypeValue),
            4 => funcT4((T4)this.SumTypeValue),
            5 => funcT5((T5)this.SumTypeValue),
            6 => funcT6((T6)this.SumTypeValue),
            7 => funcT7((T7)this.SumTypeValue),
            8 => funcT8((T8)this.SumTypeValue),
            9 => funcT9((T9)this.SumTypeValue),
            _ => throw new NotSupportedException(
                    $"BUG: sum type index {this.SumTypeValueIndex} used for sum type of two values"),
        };
    }
}
