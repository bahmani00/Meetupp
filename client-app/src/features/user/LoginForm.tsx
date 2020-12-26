import React, { useContext } from 'react';
import { Form as FinalForm, Field } from 'react-final-form';
import { Form, Button } from 'semantic-ui-react';
import TextInput from '../../app/common/form/TextInput';
import { RootStoreContext } from '../../app/stores/rootStore';
import { IUserFormValues } from '../../app/models/user';
import { FORM_ERROR } from 'final-form';
const LoginForm = () => {
  const rootStore = useContext(RootStoreContext);
  const {login} = rootStore.userStore;

  return (
    <FinalForm
      onSubmit={(values: IUserFormValues) =>
        login(values).catch(error => ({
          [FORM_ERROR]: error
        }))
      }
      render={({
        handleSubmit,
        submitting,
        form
      }) => (
      <Form onSubmit={handleSubmit}>
        <Field name='email' component={TextInput} placeholder='Email'/>
          <Field
            name='password'
            component={TextInput}
            placeholder='Password'
            type='password'
          />
          <Button
            loading={submitting}
            color='teal'
            content='Login'
            fluid
          />
	        <pre>{JSON.stringify(form.getState(), null, 2)}</pre>
      </Form>
    )}

    />
  );
};

export default LoginForm;