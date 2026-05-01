export function EmptyFlagsPlaceholder() {
  return (
    <div className="empty-flags">
      <div className="empty-flags-icon" aria-hidden>
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
          <path d="M4 7h16M4 12h10M4 17h16" strokeLinecap="round" />
        </svg>
      </div>
      <h3>No flags yet</h3>
      <p>Use the form above to add your first flag for this account.</p>
    </div>
  );
}
