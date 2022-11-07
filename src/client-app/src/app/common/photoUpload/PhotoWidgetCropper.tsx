import React, { useRef } from 'react';
import Cropper from 'react-cropper';
import 'cropperjs/dist/cropper.css';

interface IProps {
  setImage: (file: Blob) => void;
  imagePreview: string;
}

const PhotoWidgetCropper: React.FC<IProps> = ({ setImage, imagePreview }) => {
  const cropper = useRef<Cropper | null>(null);

  const cropImage = () => {
    if (
      cropper.current && cropper.current.cropper &&
      typeof cropper.current.cropper.getCroppedCanvas === 'undefined'
    ) {
      return;
    }
    cropper &&
      cropper.current &&
      cropper.current.cropper.getCroppedCanvas().toBlob((blob: any) => {
        setImage(blob);
      }, 'image/jpeg');
  };

  return (
    <Cropper
      ref={cropper}
      src={imagePreview}
      style={{ height: 200, width: '100%' }}
      // Cropper.js options
      aspectRatio={1 / 1}
      preview='.img-preview'
      guides={false}
      viewMode={1}
      dragMode='move'
      scalable={true}
      cropBoxMovable={true}
      cropBoxResizable={true}
      crop={cropImage}
    />
  );
};

export default PhotoWidgetCropper;
