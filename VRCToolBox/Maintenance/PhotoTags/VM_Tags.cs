using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.PhotoTags
{
    public class VM_Tags : ViewModelBase
    {
        private M_DataAccessorPhotoTag _mdaTag;
        public ReadOnlyReactiveCollection<VM_PhotoTag> Tags { get; }
        public ReadOnlyReactiveCollection<VM_PhotoData> Pictures { get; }
        public ReactivePropertySlim<int> SelectIndex { get; } = new ReactivePropertySlim<int>(-1);
        [Required(ErrorMessage ="名前は必須です。")]
        public ReactiveProperty<string> TagName { get; } = new ReactiveProperty<string>();

        public ReadOnlyReactivePropertySlim<string?> ErrorMessage { get; }

        public ReactivePropertySlim<string> SearchText { get; } = new ReactivePropertySlim<string>();
        public ReactiveCommand RenewCommand { get; } = new ReactiveCommand();

        public AsyncReactiveCommand SaveTagAsync { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand DeleteTagAsync { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand SearchTagsByNameAsyncCommand { get; } = new AsyncReactiveCommand();

        public VM_Tags() : this(new M_DataAccessorPhotoTag()) { }
        public VM_Tags(M_DataAccessorPhotoTag accessorPhotoTag)
        {
            _mdaTag = accessorPhotoTag;

            Tags = _mdaTag.PhotoTags.ToReadOnlyReactiveCollection(t => new VM_PhotoTag(t)).AddTo(_compositeDisposable);
            Pictures = _mdaTag.Tag.TagedPhotos.ToReadOnlyReactiveCollection(p => new VM_PhotoData(p)).AddTo(_compositeDisposable);

            TagName = _mdaTag.Tag.TagName.ToReactivePropertyAsSynchronized(t => t.Value, ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError, true).
                                          SetValidateAttribute(() => TagName).
                                          AddTo(_compositeDisposable);
            ErrorMessage = TagName.ObserveErrorChanged.Select(e => e?.Cast<string>().FirstOrDefault()).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);

            SearchText.Subscribe(async n => await _mdaTag.SearchTagsAsync(n)).AddTo(_compositeDisposable);

            SelectIndex.Where(i => i >= 0).Subscribe(i => _mdaTag.SelectTagFromCollection(i)).AddTo(_compositeDisposable);
            
            SaveTagAsync.Subscribe(_ => _mdaTag.SaveTagAsync()).AddTo(_compositeDisposable);
            DeleteTagAsync.Subscribe(_ => _mdaTag.DeleteTagAsync(SelectIndex.Value)).AddTo(_compositeDisposable);
            RenewCommand.Subscribe(_ => _mdaTag.ClearTag()).AddTo(_compositeDisposable);
            SearchTagsByNameAsyncCommand.Subscribe(_ => _mdaTag.SearchTagsAsync()).AddTo(_compositeDisposable);
        }
    }
}
