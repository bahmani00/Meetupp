import React, { Fragment, useState, useEffect } from 'react';
import { Header, Grid, Button } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';
import PhotoWidgetDropzone from './PhotoWidgetDropzone';
import PhotoWidgetCropper from './PhotoWidgetCropper';


const PhotoUploadWidget = () => (
    <Fragment>
      <Grid>
        <Grid.Column width={4}>
          <Header color='teal' sub content='Step 1 - Add Photo' />
          <PhotoWidgetDropzone />
        </Grid.Column>
        <Grid.Column width={1} />
        <Grid.Column width={4}>
          <Header sub color='teal' content='Step 2 - Resize image' />
            <PhotoWidgetCropper
            />
        </Grid.Column>
        <Grid.Column width={1} />
        <Grid.Column width={4}>
          <Header sub color='teal' content='Step 3 - Preview & Upload' />
            <Fragment>
              <div
                className='img-preview'
                style={{ minHeight: '200px', overflow: 'hidden' }}
              />
              <Button.Group widths={2}>
                <Button
                  positive
                  icon='check'
                />
                <Button
                  icon='close'
                />
              </Button.Group>
            </Fragment>
        </Grid.Column>
      </Grid>
    </Fragment>
  );

export default observer(PhotoUploadWidget);
