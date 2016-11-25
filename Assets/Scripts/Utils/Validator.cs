using System;
using System.Collections.Generic;

public class Validator {

    private Dictionary<Func<bool>, string> validators;
    private Action<String> showErrorMessage;

    public Validator(Action<String> showErrorMessage) {
        this.showErrorMessage = showErrorMessage;
    }

    public void AddValidator(Func<bool> validationFunc, string message) {
        if (validators == null) { validators = new Dictionary<Func<bool>, string>(); }
        if (validators.ContainsKey(validationFunc)) { return; }

        validators.Add(validationFunc, message);
    }

    public void SetValidators(Dictionary<Func<bool>, string> validators) {
        this.validators = validators;
    }

    public bool PerformValidate() {
        foreach (Func<bool> v in validators.Keys) {
            if (!validate(v, validators[v])) return false;
        }
        return true;
    }

    private bool validate(Func<bool> validator, string message) {
        var valid = validator.Invoke();
        if (!valid) { showErrorMessage(message); }
        return valid;
    }
}