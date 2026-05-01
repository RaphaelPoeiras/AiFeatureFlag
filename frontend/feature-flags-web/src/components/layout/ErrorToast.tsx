type ErrorToastProps = {
  message: string | null;
  onDismiss: () => void;
};

export function ErrorToast({ message, onDismiss }: ErrorToastProps) {
  if (!message) return null;

  return (
    <button type="button" className="toast-error" role="alert" title="Dismiss" onClick={onDismiss}>
      {message}
    </button>
  );
}
