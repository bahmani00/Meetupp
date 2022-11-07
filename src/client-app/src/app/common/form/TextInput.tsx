import React from 'react';
import { FieldRenderProps } from 'react-final-form';
import { FormFieldProps, Form, Label } from 'semantic-ui-react';

//using HTMLElement rather HTMLInputElement to avoid TS component warning
//https://github.com/final-form/react-final-form/issues/687 
interface IProps extends FieldRenderProps<string, HTMLElement>, FormFieldProps {
}
const TextInput: React.FC<IProps> = ({
  input,
  width,
  type,
  placeholder,
  meta: { touched, error } //https://final-form.org/docs/react-final-form/types/FieldRenderProps
}) => { //{...input} to have default properties(onblur,...) been passed
  return (
    <Form.Field error={touched && !!error} type={type} width={width}>
      <input {...input} placeholder={placeholder} /> 
      {touched && error && (
        <Label basic color='red'> 
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default TextInput;
